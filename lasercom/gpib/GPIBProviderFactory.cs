
namespace lasercom.gpib
{
    /// <summary>
    /// Instantiate concrete GPIB providers from parameters.
    /// </summary>
    public class GpibProviderFactory
    {

        public static IGpibProvider CreateGPIBProvider(GpibProviderParameters p)
        {
            return (IGpibProvider)System.Activator.CreateInstance(p.Type, p);
        }

        public static GpibProviderParameters CreateGPIBProviderParameters(GpibProviderParameters p)
        {
            GpibProviderParameters q = new GpibProviderParameters
            {
                Type = p.Type,
                Name = p.Name,
                PortName = p.PortName,
                Timeout = p.Timeout,
                BoardNumber = p.BoardNumber
            };
            return q;
        }

    }
}
