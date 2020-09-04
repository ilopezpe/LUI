using ATMCD32CS;

namespace lasercom.objects
{
    public sealed class AndorFactory
    {
        static volatile AndorSDK _AndorSdkInstance;
        static readonly object AndorSdkLock = new object();

        AndorFactory()
        {
        }

        public static AndorSDK AndorSdkInstance
        {
            get
            {
                if (_AndorSdkInstance == null)
                    lock (AndorSdkLock)
                    {
                        if (_AndorSdkInstance == null) _AndorSdkInstance = new AndorSDK();
                    }

                return _AndorSdkInstance;
            }
        }
    }
}