using lasercom.objects;
using log4net;

namespace lasercom.gpib
{
    /// <summary>
    /// Base class for all GPIB providers.
    /// </summary>
    public abstract class AbstractGpibProvider : LuiObject<GpibProviderParameters>, IGpibProvider
    {
#pragma warning disable CS0108 // 'AbstractGpibProvider.Log' hides inherited member 'LuiObject.Log'. Use the new keyword if hiding was intended.
        protected static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#pragma warning restore CS0108 // 'AbstractGpibProvider.Log' hides inherited member 'LuiObject.Log'. Use the new keyword if hiding was intended.

        abstract public void LoggedWrite(byte address, string command);

        abstract public string LoggedQuery(byte address, string command);

        public override void Update(GpibProviderParameters p)
        {
            throw new System.NotImplementedException();
        }
    }
}
