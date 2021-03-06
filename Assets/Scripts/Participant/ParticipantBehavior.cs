﻿using System;
using System.Collections;
using Assets.Scripts.DataCollection.Physiological;
using FeedScreen.Experiment;
using Menu_Navigation.Button_Logic;
using Networking;
using Tobii.Plugins;
using UnityEngine;
using UnityEngine.Networking;

namespace Participant
{
    /// <summary>
    /// The Participant Behavior class is responsible for managing the lifecycle of the Participant.
    /// 
    /// Participant behavior acts as a wrapper for the Participant class to integrate into Unity.
    /// 
    /// </summary>
    [RequireComponent(typeof(Recorder))]
    public class ParticipantBehavior : MonoBehaviour
    {
        public static ParticipantBehavior Instance;

        public static Participant Participant;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this)
                Destroy(gameObject);

            if (!gameObject.GetComponentInChildren<Recorder>())
            {

                Recorder recorder = gameObject.AddComponent<Recorder>();
                recorder.Rate = 1;
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// This is a wrapper for the NewParticipantRequest coroutine
        /// </summary>
        /// <param name="group"></param>
        public void MakeNewParicipant(int group)
        {
            var participantData = new ParticipantData
            {
                // TODO Create GUID and upload that to the database instead of waiting for the response from the server.
                Group = group,
                ProctorName = GroupSelection.InputField.text
            };

            Debug.Log(string.Format(
                "Attempting to make New Participant: Transparency={0} Adaptive={1} Proctor={2}",
                participantData.Transparent, participantData.Adaptive,
                GroupSelection.InputField.text));

            StartCoroutine(NewParticipantRequest(participantData));


        }

        public void MakeNewParicipant(int group, int currentTimeline, int currentMission)
        {
            var participantData = new ParticipantData
            {
                // TODO Create GUID and upload that to the database instead of waiting for the response from the server.
                Group = group,
                ProctorName = GroupSelection.InputField.text
            };

            Debug.Log(string.Format(
                "Attempting to make New Participant: Transparency={0} Adaptive={1} Proctor={2}",
                participantData.Transparent, participantData.Adaptive,
                GroupSelection.InputField.text));

            StartCoroutine(NewParticipantRequest(participantData, currentTimeline, currentMission));
        }

        public IEnumerator NewParticipantRequest(ParticipantData data, int currentTimeline, int currentMission)
        {
            var form = new WWWForm();
            form.AddField("adaptive", data.Adaptive ? "1" : "0");
            form.AddField("transparent", data.Transparent ? "1" : "0");
            form.AddField("group_number", data.Group);
            form.AddField("proctor_name", data.ProctorName);

            var www = UnityWebRequest.Post(ServerURL.INSERT_PARTICIPANT, form);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                SceneFlowController.LoadErrorScene();
            }
            else
            {
                var result = JSON.Parse(www.downloadHandler.text);

                if (result["failed"].AsBool)
                    SceneFlowController.LoadErrorScene();
                else
                    data.Id = result["data"].AsInt;


                Participant = new Participant
                {
                    Data = data,
                    CurrentMission = currentMission
                };

                Debug.Log(string.Format(
                    "New Participant Made: Transparency={0} Adaptive={1} Proctor={2}",
                    Participant.Data.Transparent, Participant.Data.Adaptive,
                    Participant.Data.ProctorName));
                EventManager.OnNewParticipantMade(new NewParticipantEventArgs
                {
                    Data = data
                });
            }
        }


        /// <summary>
        /// Attempts to create a participant by sending an HTTP request to server and waiting for the response.
        /// </summary>
        /// <param name="data">
        /// The metadata of the participant such as their group number and proctor name.
        /// </param>
        /// <returns></returns>
        private IEnumerator NewParticipantRequest(ParticipantData data)
        {
            var form = new WWWForm();
            form.AddField("adaptive", data.Adaptive ? "1" : "0");
            form.AddField("transparent", data.Transparent ? "1" : "0");
            form.AddField("group_number", data.Group);
            form.AddField("proctor_name", data.ProctorName);

            var www = UnityWebRequest.Post(ServerURL.INSERT_PARTICIPANT, form);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                SceneFlowController.LoadErrorScene();
            }
            else
            {
                var result = JSON.Parse(www.downloadHandler.text);
                Debug.Log(result);
                if (result["failed"].AsBool)
                    SceneFlowController.LoadErrorScene();
                else
                    data.Id = result["data"].AsInt;


                Participant = new Participant
                {
                    Data = data,
                    CurrentMission = 1,
                    CurrentSurvey = 1
                };

                Debug.Log(string.Format(
                    "New Participant Made: Transparency={0} Adaptive={1} Proctor={2}, ID={3}",
                    Participant.Data.Transparent, Participant.Data.Adaptive,
                    Participant.Data.ProctorName,
                    Participant.Data.Id));
                EventManager.OnNewParticipantMade(new NewParticipantEventArgs
                {
                    Data = data
                });
            }
        }
    }
}