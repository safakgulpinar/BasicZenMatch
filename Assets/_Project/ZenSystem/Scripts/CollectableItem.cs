using DG.Tweening;
using Lean.Touch;
using UnityEngine;

namespace _Project.ZenSystem.Scripts
{
    public class CollectableItem : MonoBehaviour
    {
        public int ItemTypeIndex = 0;
        
        private LeanSelectableByFinger _leanSelectableByFinger;

        [SerializeField] private Transform modelTransform;

        private void Awake()
        {
            _leanSelectableByFinger = GetComponent<LeanSelectableByFinger>();
        }

        public void Selected()
        {
            _leanSelectableByFinger.enabled = false;
            
            // Haptic?, Sound?, Particle?
            
            ZenManager.Instance.CheckAEmptySlot(this);
        }
        
        public void GoToSlot(Vector3 slotPos, bool placeAnim = true)
        {
            transform.DOKill(true);

            transform.DORotate(Vector3.zero, .15f);
            modelTransform.DOLocalRotate(new Vector3(50, 0, 0), .2f);
            
            transform.DOMove(slotPos, .2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (placeAnim)
                {
                    modelTransform.DOPunchScale(modelTransform.localScale * 0.35f, 0.175f, 1, 0.5f)
                        .SetEase(Ease.InOutCirc)
                        .SetLoops(1, LoopType.Yoyo);
                    
                    //Sound?, Particle?, Haptic?
                }
            });
        }
        
        public void MergeEndScaleAnim()
        {
            transform.DOScale(Vector3.zero, .15f).SetEase(Ease.InBounce).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}