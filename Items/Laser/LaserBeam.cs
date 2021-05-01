using UnityEngine;

namespace Bloops.GridFramework.Items.Laser
{
    public class LaserBeam : ItemBase
    {
        private Vector2Int orientation;
    
        [SerializeField] private GameObject verticalBeamVisuals;

        [SerializeField] private GameObject horizontalBeamVisuals;

        protected new void OnEnable()
        {
            base.OnEnable();
            ItemInitiation();
        }

        protected override void OnGameReady()
        {
            //initialize if this object is spawned after the game started but before its ready. (like a spawner spawning this on init).
            //Thats because this can get called BEFORE this object gets its first OnEnable function called, which will happen frame 2, as its spawned at the end of frame 1.
            //im rolling my own puzzleManager to prevent exactly this kinds of race conditions, but HEY. its FINE.
        
            //on game ready is for doing things like 
            
            //I found the problem when i was testing blocks spawning on items that moved them.
            if (_node == null)
            {
                ItemInitiation();
            }

            base.OnGameReady();
        }

        public void SetAngle(MirrorAngle angle)
        {
            //turn RightUp into like, the laser at a 45 degree angle.
            verticalBeamVisuals.SetActive(true);
            horizontalBeamVisuals.SetActive(true);
        }

        // Start is called before the first frame update
        public void SetOrientation(Vector3Int dir)
        {
            this.orientation = new Vector2Int(Mathf.Abs(dir.x),Mathf.Abs(dir.y));;
            UpdateOrientationVisuals();
        }

        private void UpdateOrientationVisuals()
        {
            verticalBeamVisuals.SetActive(orientation.y == 1);
            horizontalBeamVisuals.SetActive(orientation.x == 1);
        }
    }
}
