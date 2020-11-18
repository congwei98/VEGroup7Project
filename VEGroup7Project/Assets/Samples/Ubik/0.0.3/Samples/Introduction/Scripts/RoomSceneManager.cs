﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ubik.Rooms;
using Ubik.Messaging;

namespace Ubik.Samples
{
    public class RoomSceneManager : MonoBehaviour
    {
        private RoomClient client;

        private void Awake()
        {
            client = GetComponentInParent<RoomClient>();
        }

        private void Start()
        {
            client.OnRoom.AddListener(OnRoom);
            client.OnJoinedRoom.AddListener(OnJoinedRoom);
        }

        private void OnJoinedRoom()
        {
            var name = client.room["scene-name"];
            if(name == null)
            {
                UpdateRoomScene(); // if we've joined a room without a scene
            }
        }

        private void OnRoom()
        {
            var name = client.room["scene-name"];
            if (name != null)
            {
                LoadSceneAsync(name);
            }
        }

        public void UpdateRoomScene()
        {
            var active = SceneManager.GetActiveScene();
            client.room["scene-name"] = active.name;
            client.room["scene-path"] = active.path;

            foreach (var item in active.GetRootGameObjects())
            {
                var info = item.GetComponentInChildren<SceneInfo>();
                if(info)
                {
                    client.room["scene-image"] = client.SetBlob(client.room.guid, info.base64image);
                    break;
                }
            }
        }

        public void ChangeScene(string name)
        {
            LoadSceneAsync(name);
        }

        private void LoadSceneAsync(string name)
        {
            if(SceneManager.GetSceneByName(name).isLoaded)
            {
                return;
            }

            SceneManager.LoadSceneAsync(name, LoadSceneMode.Single).completed +=
                (operation) =>
                {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
                    UpdateRoomScene();
                };
        }

        public static void ChangeScene(MonoBehaviour caller, string sceneName)
        {
            var manager = FindSceneManager(NetworkScene.FindNetworkScene(caller));
            manager.ChangeScene(sceneName);
        }

        private static RoomSceneManager FindSceneManager(NetworkScene scene)
        {
            var manager = scene.GetComponentInChildren<RoomSceneManager>();
            Debug.Assert(manager != null, $"Cannot find RoomSceneManager Component for {scene}. Ensure a RoomSceneManager Component has been added.");
            return manager;
        }
    }
}