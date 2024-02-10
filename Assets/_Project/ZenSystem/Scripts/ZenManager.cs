using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _Project.ZenSystem.Scripts
{
    public class ZenManager : MonoBehaviour
    {
        public static ZenManager Instance { get; private set; }
        
        private const int MergeCount = 3;
        private readonly List<CollectableItem> _collectableItems = new ();
        
        [SerializeField] private List<ZenSlot> zenSlots;
        
        #region Returners

        private ZenSlot GetEmptySlot()
        {
            foreach (var zenSlot in zenSlots)
            {
                if (zenSlot.GetCurrentCollectableItem() == null)
                {
                    return zenSlot;
                }
            }

            return null;
        }
        #endregion
        
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

        public void CheckAEmptySlot(CollectableItem collectableItem)
        {
            if(GetEmptySlot() == null)
            {
                //Do something
                //-Level failed
                //-Touch false
                return;
            }
            
            for (var i = 0; i < zenSlots.Count; i++)
            {
                var zenSlot = zenSlots[i];
                var zenSlotCurrentCollectableItem = zenSlot.GetCurrentCollectableItem();

                if (zenSlotCurrentCollectableItem != null && zenSlotCurrentCollectableItem.ItemTypeIndex == collectableItem.ItemTypeIndex)
                {
                    if (zenSlots[i + 1].GetCurrentCollectableItem() == null)
                    {
                        zenSlots[i + 1].SlotFull(collectableItem);
                        //particle?
                        break;
                    }
                    else
                    {
                        ExcludeSlots(i+1);
                        zenSlots[i + 1].SlotFull(collectableItem);
                        //particle?
                        break;
                    }
                }
                else if (zenSlotCurrentCollectableItem == null)
                {
                    zenSlot.SlotFull(collectableItem);
                    //particle?
                    break;
                }
            }
            CheckMerge(collectableItem.ItemTypeIndex);
        }

        
        private void ExcludeSlots(int excludeIndex)
        {
            _collectableItems.Clear();
            
            for (var i = excludeIndex; i < zenSlots.Count; i++)
            {
                if (zenSlots[i].GetCurrentCollectableItem() == null) continue;
                _collectableItems.Add(zenSlots[i].GetCurrentCollectableItem());
                zenSlots[i].SlotEmpty();
            }

            for (var i = 0; i < _collectableItems.Count; i++)
            {
                var targetIndex = excludeIndex + 1 + i;
                if (targetIndex < zenSlots.Count)
                {
                    zenSlots[targetIndex].SlotFull(_collectableItems[i], false);
                }
            }
        }

        private void CheckMerge(int collectableItemTypeIndex)
        {
            var willBeMergedCollectableItems = new List<CollectableItem>();
            
            var willBeEmptyZenSlots = new List<ZenSlot>();
            

            for (var i = 0; i < zenSlots.Count; i++)
            {
                var zenSlot = zenSlots[i];

                var currentCollectableItem = zenSlot.GetCurrentCollectableItem();
                
                if (currentCollectableItem != null && currentCollectableItem.ItemTypeIndex == collectableItemTypeIndex)
                {
                    willBeMergedCollectableItems.Add(currentCollectableItem);
                    willBeEmptyZenSlots.Add(zenSlot);
                }
            }
            
            if(willBeMergedCollectableItems.Count >= MergeCount)
            {
                foreach (var zenSlot in willBeEmptyZenSlots)
                {
                    zenSlot.SlotEmpty();
                }
                MergeCollectableItems(willBeMergedCollectableItems);
                
            }
        }

        private void MergeCollectableItems(IReadOnlyList<CollectableItem> willBeMergedCollectableItems)
        {
            DOVirtual.DelayedCall(0.25f, () =>
            {
                var middleIndex = willBeMergedCollectableItems.Count / 2;

                for (var i = 0; i < willBeMergedCollectableItems.Count; i++)
                {
                    var willBeMergedCollectableItem = willBeMergedCollectableItems[i];
                    var collectableItemTransform = willBeMergedCollectableItem.transform;
                    
                    var collectableItemPos = collectableItemTransform.position;
                    var targetPos = new Vector3(collectableItemPos.x, collectableItemPos.y + 2f, collectableItemPos.z + 2); //Set the offset of the merge position here.
                    collectableItemTransform.DOMove(targetPos, .15f).OnComplete(() =>
                    {
                        collectableItemTransform.DOMoveX(willBeMergedCollectableItems[middleIndex].transform.position.x, .18f)
                            .SetEase(Ease.OutBounce);
                    });
                }
                
                SortSlots();
                
                DOVirtual.DelayedCall(0.35f, () =>
                {
                    foreach (var collectableItem in willBeMergedCollectableItems)
                    {
                        collectableItem.MergeEndScaleAnim();
                    }
                    //Sound?, Haptic?, Particle?
                });
            });
        }

        private void SortSlots()
        {
            var willBeSortedCollectableItems = new List<CollectableItem>();

            for (var i = 0; i < zenSlots.Count; i++)
            {
                var zenSlot = zenSlots[i];
                var currentCollectableItem = zenSlot.GetCurrentCollectableItem();

                if (currentCollectableItem == null) continue;
                willBeSortedCollectableItems.Add(currentCollectableItem);
                zenSlot.SlotEmpty();
            }

            for (var i = 0; i < willBeSortedCollectableItems.Count; i++)
            {
                zenSlots[i].SlotFull(willBeSortedCollectableItems[i]);
            }
        }
        
        private void ResetSlots()
        {
            foreach (var zenSlot in zenSlots)
            {
                zenSlot.SlotEmpty();
            }
        }
    }
}