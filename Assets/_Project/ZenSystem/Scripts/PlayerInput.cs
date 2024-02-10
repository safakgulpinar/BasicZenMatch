using UnityEngine;

namespace _Project.ZenSystem.Scripts
{
    public class PlayerInput : MonoBehaviour
    {
        private static PlayerInput Instance { get; set; }

        [SerializeField] private GameObject leanTouchGo;
        
        private void Awake() 
        {
            //Basic singleton pattern
            if (Instance != null && Instance != this) 
            { 
                Destroy(this);
                return;
            } 
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        public void TouchActive(bool active)
        {
            leanTouchGo.SetActive(active);
        }
    }
}