namespace UnityEngine
{
    public class WaitForFrames : CustomYieldInstruction
    {
        private int framesToWait;

        public WaitForFrames(int frames)
        {
            framesToWait = frames;
        }

        public override bool keepWaiting
        {
            get
            {
                if (framesToWait > 0)
                {
                    framesToWait--;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}