using UnityEngine;

namespace _Project.ZenSystem.Scripts
{
    public class ZenSlot : MonoBehaviour
    {
        private CollectableItem _currentCollectableItem;
        
        #region Returners
        
        public CollectableItem GetCurrentCollectableItem()
        {
            return _currentCollectableItem;
        }
        
        #endregion

        public void SlotFull(CollectableItem collectableItem, bool placeAnim = true)
        {
            _currentCollectableItem = collectableItem;
            _currentCollectableItem.GoToSlot(transform.position + transform.up * 0.35f, placeAnim);
        }
        public void SlotEmpty()
        {
            _currentCollectableItem = null;
        }
    }
}