﻿namespace HexagonUtils {
    using System.Collections.Generic;
    using UnityEngine;

    public class SystemManager : MonoBehaviour {

        public static SystemManager _instance;

        public GameObject playerPrefab;
        public MapGenerator mapGenerator;
        public CameraManager cameraManager;
        public BattleManager battleManager;

        private Player m_player;

        private void Start()
        {
            if (!_instance)
            {
                _instance = this;
            }
            mapGenerator.GenerateMap();
            Vector2 spawnPoint = mapGenerator.GetSpawnPoint();
            GameObject go = Instantiate(playerPrefab, new Vector2((spawnPoint.x + spawnPoint.y % 2 * 0.5f) * 1.25f, spawnPoint.y * 1.0875f), Quaternion.identity);
            m_player = go.GetComponent<Player>();
            m_player.currentPosition = new HexCoordinate(spawnPoint);
            Debug.Log(spawnPoint);
            cameraManager.target = go.transform;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && m_player.state == EntityState.IDLE)
                Interaction();
        }

        public void OnClickMove(HexCoordinate hex)
        {
            if (!mapGenerator.dirGridEntity[hex].isBlocked)
                m_player.PlayerNavigate(hex);
        }

        public void Interaction()
        {

        }

    }
}