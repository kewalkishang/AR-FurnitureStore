//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;
    using UnityEngine.UI;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

    /// <summary>
    /// Controls the HelloAR example.
    /// </summary>
    public class HelloARController : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        /// 

        ///To keep track of the last clicked object
        public static GameObject SelectedModel = null;

        public static bool move = false;

        /// UI elements
        public GameObject editPanel;
        public GameObject SelectPanel;
        public GameObject scalerslider;
        public GameObject rotateslider;
        public GameObject ColorSliders;
        public Camera FirstPersonCamera;
        string msg = " ";
        string touched = " ";
        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject DetectedPlanePrefab;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject AndyPlanePrefab;
        public GameObject[] furniture; 
        /// <summary>
        /// A model to place when a raycast from a user touch hits a feature point.
        /// </summary>
        public GameObject AndyPointPrefab;

        /// <summary>
        /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
        /// </summary>
        public GameObject SearchingForPlaneUI;

        /// <summary>
        /// The rotation in degrees need to apply to model when the Andy model is placed.
        /// </summary>
        private const float k_ModelRotation = 180.0f;

        /// <summary>
        /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            _UpdateApplicationLifecycle();

            // Hide snackbar when currently tracking at least one plane.
            Session.GetTrackables<DetectedPlane>(m_AllPlanes);
            bool showSearchingUI = true;
            for (int i = 0; i < m_AllPlanes.Count; i++)
            {
                if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
                {
                    showSearchingUI = false;

                    
                    break;
                }
            }

            SearchingForPlaneUI.SetActive(showSearchingUI);
            if (editPanel.activeInHierarchy == false)
            {
                SelectPanel.SetActive(!showSearchingUI);
            }
            // If the player has not touched the screen, we are done with this update.
            if (move == false)
            {
                Touch touch;
                if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
                {
                    return;
                }

                // Raycast against the location the player touched to search for planes.
                TrackableHit hit;
                TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                    TrackableHitFlags.FeaturePointWithSurfaceNormal;

                RaycastHit hit2;
                Ray raycast = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(raycast, out hit2, Mathf.Infinity))
                {


                    if (SelectedModel != null)
                    {
                        SelectedModel.transform.Find("highlighter").gameObject.SetActive(false);
                    }

                    SelectedModel = hit2.transform.gameObject;
                    SelectedModel.transform.Find("highlighter").gameObject.SetActive(true);
                    EnableEditing();


                }
                else
                {
                    if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit) && (editPanel.activeInHierarchy == false))
                    {
                        // Use hit pose and camera pose to check if hittest is from the
                        // back of the plane, if it is, no need to create the anchor.
                        if ((hit.Trackable is DetectedPlane) &&
                            Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                                hit.Pose.rotation * Vector3.up) < 0)
                        {
                            Debug.Log("Hit at back of the current DetectedPlane");
                        }
                        else
                        {
                            // Choose the Andy model for the Trackable that got hit.
                            GameObject prefab;
                            if (hit.Trackable is FeaturePoint)
                            {
                                prefab = AndyPointPrefab;
                            }
                            else
                            {
                                prefab = AndyPlanePrefab;
                            }

                            // Instantiate Andy model at the hit pose.
                            var andyObject = Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);

                            // Compensate for the hitPose rotation facing away from the raycast (i.e. camera).
                            andyObject.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

                            // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                            // world evolves.
                            var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                            // Make Andy model a child of the anchor.
                            andyObject.transform.parent = anchor.transform;
                        }
                    }
                }
            }
             else
            {
                msg = "in moving ";
                Touch touch;
                if (Input.touchCount == 1 && (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
                {
                    msg = "ssasd ";
                    TrackableHit hit;
                    TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                   TrackableHitFlags.FeaturePointWithSurfaceNormal;

                    if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
                    {
                        msg = "hot raycast ";
                        // Use hit pose and camera pose to check if hittest is from the
                        // back of the plane, if it is, no need to create the anchor.
                        if (hit.Trackable is DetectedPlane)
                        {
                            SelectedModel.transform.position = hit.Pose.position;
                        }
                        
                    }
                }
            }

        }

        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
        private void _UpdateApplicationLifecycle()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }
        void OnGUI()
        {

            GUI.Label(new Rect(10, 10, 1000, 202), "count" + msg);

            GUI.Label(new Rect(10, 20, 1000, 202), "touched" + touched);
        }

        public void enableLamp()
        {
            AndyPlanePrefab = furniture[4];
            msg = "pressed lamp";
        }
        public void enableDesk()
        {
            AndyPlanePrefab = furniture[0];
            msg = "pressed desk";
        }
        public void enableChair()
        {
            AndyPlanePrefab = furniture[1];
            msg = "pressed chair";
        }
        public void enableTV()
        {
            AndyPlanePrefab = furniture[3];
        }
        public void enableDrawer()
        {
            AndyPlanePrefab = furniture[2];
        }

        public void EnableEditing()
        {
            SelectPanel.SetActive(false);
            editPanel.SetActive(true);
            ColorSliders.SetActive(false);
        }

        public void QuitEditing()
        {
            SelectedModel.transform.Find("highlighter").gameObject.SetActive(false);
            SelectedModel = null;
            ColorSliders.SetActive(false);
            SelectPanel.SetActive(true);
            editPanel.SetActive(false);
            move = false;
        }
        public void EnableRotate()
        {
            ColorSliders.SetActive(false);
            rotateslider.SetActive(true);
            scalerslider.SetActive(false);
            move = false;
           
        }
        public void EnableScaler()
        {
            ColorSliders.SetActive(false);
            scalerslider.SetActive(true);
            rotateslider.SetActive(false);
            move = false;
            
        }
        public void Scale(Slider sl)
        { float val = sl.value+1;
            Vector3 sc = new Vector3(val,val,val);
            Debug.Log("scA" + sl.value);
            SelectedModel.transform.localScale = sc;
            // Debug.Log("MODEL" + ModelCount);

        }

        public void Rotate(Slider sl)
        {
            Vector3 sc = new Vector3(0, sl.value*360, 0);
            Debug.Log("scA" + sl.value);
            SelectedModel.transform.rotation = Quaternion.Euler(sc);
            Debug.Log("scale" + transform.localScale);
            
        }

        public void DeleteObject()
        {
            Destroy(SelectedModel.transform.parent.gameObject);
            SelectedModel = null;
            SelectPanel.SetActive(true);
            editPanel.SetActive(false);
            ColorSliders.SetActive(false);
        }
        public void MoveObject()
        {
            move = true;
            SelectPanel.SetActive(false);
            ColorSliders.SetActive(false);
            rotateslider.SetActive(false);
        }


        public void EnableColor()
        {
            move = false;
            scalerslider.SetActive(false);
            rotateslider.SetActive(false);
            ColorSliders.SetActive(true);

            Material mat = new Material(Shader.Find("Standard"));
            GameObject mod = SelectedModel.transform.Find("model").gameObject;
            GameObject ss = mod.transform.Find("color").gameObject;
            foreach (Transform child in ss.transform)
            {
                Renderer rend = child.gameObject.GetComponent<Renderer>();
                rend.material = mat;
                rend.material.SetColor("_Color", new Color(0, 0, 0));
                // Debug.Log("color" + ;
            }
        }

        public void ChangeRed(Slider s)
        {
            foreach (Transform child in SelectedModel.transform.Find("model").gameObject.transform.Find("color").gameObject.transform)
            {
                Renderer rend = child.gameObject.GetComponent<Renderer>();
                rend.material.SetColor("_Color", new Color(s.value, rend.material.color[1], rend.material.color[2]));
            }
            //   gameObject
        }

        public void ChangeGreen(Slider s)
        {
            foreach (Transform child in SelectedModel.transform.Find("model").gameObject.transform.Find("color").gameObject.transform)
            {
                Renderer rend = child.gameObject.GetComponent<Renderer>();
                rend.material.SetColor("_Color", new Color(rend.material.color[0], s.value, rend.material.color[2]));
            }
        }

        public void ChangeBlue(Slider s)
        {
            foreach (Transform child in SelectedModel.transform.Find("model").gameObject.transform.Find("color").gameObject.transform)
            {
                Renderer rend = child.gameObject.GetComponent<Renderer>();
                rend.material.SetColor("_Color", new Color(rend.material.color[0], rend.material.color[1], s.value));
            }
        }

    }
}
