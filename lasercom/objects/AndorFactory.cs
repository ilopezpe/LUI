using ATMCD32CS;

namespace lasercom.objects
{
    public sealed class AndorFactory
    {
        private static volatile AndorSDK _AndorSdkInstance;
        private static object AndorSdkLock = new object();

        private AndorFactory() { }

        public static AndorSDK AndorSdkInstance
        {
            get
            {
                if (_AndorSdkInstance == null)
                {
                    lock (AndorSdkLock)
                    {
                        if (_AndorSdkInstance == null)
                        {
                            _AndorSdkInstance = new AndorSDK();
                        }
                    }
                }
                return _AndorSdkInstance;
            }
        }
    }
}
