using Extensions;
using LuiHardware;
using LuiHardware.beamflags;
using LuiHardware.camera;
using LuiHardware.ddg;
using LuiHardware.objects;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using LUI.tabs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

//  <summary>
//      Class for managing LUI XML config files.
//  </summary>

namespace LUI.config
{
    [XmlRoot("LuiConfig")]
    public class LuiConfig : IXmlSerializable, IDisposable
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public LuiConfig()
            : this(Constants.DefaultConfigFileLocation)
        {
        }

        public LuiConfig(string configFile)
        {
            DryRun = false;
            ConfigFile = configFile;
            LogFile = Constants.DefaultLogFileLocation;
            LogLevel = Constants.DefaultLogLevel;

            Saved = false;

            TabSettings = new Dictionary<string, Dictionary<string, string>>();

            // Prepopulate tab settings using all LuiTab subclasses.
            foreach (var type in typeof(LuiTab).GetSubclasses(true))
            {
                TabSettings.Add(type.Name, new Dictionary<string, string>());
            }

            LuiObjectTableIndex = new Dictionary<Type, Dictionary<LuiObjectParameters, ILuiObject>>();

            // Prepopulate parameter lists using all concrete LuiObjectParameters subclasses.
            foreach (var type in typeof(LuiObjectParameters).GetSubclasses(true))
            {
                LuiObjectTableIndex.Add(type, new Dictionary<LuiObjectParameters, ILuiObject>());
            }
        }

        public Dictionary<Type, Dictionary<LuiObjectParameters, ILuiObject>> LuiObjectTableIndex { get; set; }

        public Dictionary<string, Dictionary<string, string>> TabSettings { get; set; }

        public bool Saved { get; private set; }

        public bool Ready => false;

        /// <summary>
        ///     Enumerates all the parameters in the object table.
        /// </summary>
        public IEnumerable<LuiObjectParameters> LuiObjectParameters
        {
            get
            {
                foreach (var subtable in LuiObjectTableIndex.Values) // List of subtables
                    foreach (var kvp in subtable) // parameter/object pair
                        yield return kvp.Key;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public XmlSchema GetSchema()
        {
            return null; // This method is deprecated, framework documentation says to return null.
        }

        public void ReadXml(XmlReader reader)
        {
            // Root element NOT handled automatically.
            reader.MoveToContent();
            var empty = reader.IsEmptyElement;
            //reader.ReadStartElement(); // Read root element.
            if (!empty)
            {
                // Application parameters.
                reader.ReadToFollowing("ApplicationParameters");
                using (var subtree = reader.ReadSubtree())
                {
                    subtree.MoveToContent();
                    while (subtree.Read())
                        if (subtree.IsStartElement())
                            typeof(LuiConfig).GetProperty(subtree.Name)
                                .SetValue(this, subtree.ReadElementContentAsString());
                }

                // LuiObjectParamters lists.
                var settings = new DataContractSerializerSettings
                {
                    PreserveObjectReferences = true,
                    KnownTypes = typeof(LuiObjectParameters).GetSubclasses(true)
                };
                var serializer = new DataContractSerializer(typeof(LuiObjectParameters), settings);

                reader.ReadToFollowing("LuiObjectParametersList");
                using (var subtree = reader.ReadSubtree())
                {
                    subtree.MoveToContent();
                    while (subtree.Read())
                    {
                        subtree.MoveToContent();
                        if (!subtree.Name.EndsWith("List") && subtree.IsStartElement())
                        {
                            var p = (LuiObjectParameters)serializer.ReadObject(subtree.ReadSubtree());
                            AddParameters(p);
                        }
                    }
                }

                // Tab settings.

                reader.ReadToFollowing("TabSettings");
                using (var subtree = reader.ReadSubtree())
                {
                    subtree.MoveToContent();
                    ISet<string> TabNames = new HashSet<string>(typeof(LuiTab).GetSubclasses(true).Select(x => x.Name));
                    string Tab = null;
                    while (subtree.Read())
                    {
                        subtree.MoveToContent();
                        if (subtree.IsStartElement())
                        {
                            if (TabNames.Contains(subtree.Name))
                            {
                                Tab = subtree.Name;
                            }
                            else
                            {
                                var Name = subtree.Name;
                                subtree.Read();
                                TabSettings[Tab].Add(Name, subtree.Value);
                            }
                        }
                    }
                }

                //reader.ReadEndElement(); // End root.
            }

            Saved = true;
        }

        public void WriteXml(XmlWriter writer)
        {
            // Root element <LuiConfig> start/end handled automatically.
            // Write the individual options.
            writer.WriteStartElement("ApplicationParameters");
            writer.WriteElementString(Util.GetPropertyName(() => ConfigFile), ConfigFile);
            writer.WriteElementString(Util.GetPropertyName(() => LogFile), LogFile);
            writer.WriteElementString(Util.GetPropertyName(() => LogLevel), LogLevel);
            writer.WriteEndElement();

            // Write the LuiObjectParameters.
            var settings = new DataContractSerializerSettings
            {
                PreserveObjectReferences = true,
                KnownTypes = typeof(LuiObjectParameters).GetSubclasses(true)
            };
            var serializer = new DataContractSerializer(typeof(LuiObjectParameters), settings);

            writer.WriteStartElement("LuiObjectParametersList");
            foreach (var kvp in LuiObjectTableIndex)
            {
                // Write list of specific LuiObjectParameters subtype.
                writer.WriteStartElement(kvp.Key.Name + "List");
                foreach (var p in kvp.Value.Keys) serializer.WriteObject(writer, p);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            // Write TabSettings.
            writer.WriteStartElement("TabSettings");
            foreach (var TabKvp in TabSettings)
            {
                // Write settings for specific LuiTab subtype.
                writer.WriteStartElement(TabKvp.Key);
                foreach (var SettingKvp in TabKvp.Value) writer.WriteElementString(SettingKvp.Key, SettingKvp.Value);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            Saved = true;
        }

        public event EventHandler ParametersChanged;

        public void ConfigureLogging()
        {
            var tracer = new TraceAppender();
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.AddAppender(tracer);

            var patternLayout = new PatternLayout
            {
                ConversionPattern = "%date [%thread] %-5level %logger - %message%newline"
            };
            patternLayout.ActivateOptions();
            tracer.Layout = patternLayout;

            var fileAppender = new FileAppender
            {
                AppendToFile = true,
                File = LogFile,
                Layout = patternLayout,
                LockingModel = new FileAppender.ExclusiveLock()
            };
            fileAppender.ActivateOptions();
            hierarchy.Root.AddAppender(fileAppender);

            hierarchy.Root.Level = hierarchy.LevelMap[LogLevel];
            hierarchy.Configured = true;
        }

        public ILuiObject GetObject(LuiObjectParameters p)
        {
            ILuiObject val;
            //LuiObjectTableIndex[p.GetType()].TryGetValue(p, out val);
            val = LuiObjectTableIndex[p.GetType()][p];
            return val;
        }

        public void SetObject(LuiObjectParameters p, ILuiObject o)
        {
            LuiObjectTableIndex[p.GetType()][p] = o;
        }

        /// <summary>
        ///     Get all the parameters of a particular type, as that type.
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <returns></returns>
        public IEnumerable<P> GetParameters<P>() where P : LuiObjectParameters<P>
        {
            return LuiObjectTableIndex[typeof(P)].Keys.AsEnumerable().Cast<P>();
        }

        /// <summary>
        ///     Get the parameters of a particular type as nongeneric parameters.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public IEnumerable<LuiObjectParameters> GetParameters(Type t)
        {
            return LuiObjectTableIndex[t].Keys.AsEnumerable();
        }

        public LuiObjectParameters GetFirstParameters(string Name)
        {
            foreach (var p in LuiObjectParameters)
                if (p.Name == Name)
                    return p;
            return null;
        }

        public LuiObjectParameters GetFirstParameters(Type t, string Name)
        {
            foreach (var p in GetParameters(t))
                if (p.Name == Name)
                    return p;
            return null;
        }

        public void AddParameters(LuiObjectParameters p)
        {
            Dictionary<LuiObjectParameters, ILuiObject> subtable;
            var found = LuiObjectTableIndex.TryGetValue(p.GetType(), out subtable);
            if (!found) LuiObjectTableIndex.Add(p.GetType(), new Dictionary<LuiObjectParameters, ILuiObject>());
            LuiObjectTableIndex[p.GetType()].Add(p, null);
        }

        public void ReplaceParameters<P>(IEnumerable<P> NewParameters) where P : LuiObjectParameters<P>
        {
            var OldParameters = LuiObjectTableIndex[typeof(P)].Keys.AsEnumerable().Cast<P>();

            // New parameters where all old parameters have different name.
            // Same as "New parameters where not any old parameters have same name."
            // I.e. Where(p => !OldParameters.Any(q => q.Name == p.Name));
            var DefinitelyNew = NewParameters.Where(p => OldParameters.All(q => q.Name != p.Name));
            // Old parameters where all new parameters have different name.
            // Adding ToList() makes a copy which can be iterated while modifying the source enumerable.
            var DefinitelyOld = OldParameters.Where(p => NewParameters.All(q => q.Name != p.Name)).ToList();

            // Dispose all definitely old entries.
            foreach (var p in DefinitelyOld)
            {
                var luiObject = LuiObjectTableIndex[p.GetType()][p];
                if (luiObject != null) luiObject.Dispose();
                LuiObjectTableIndex[p.GetType()].Remove(p); // Only legal because DefinitelyOld copied with ToList().
            }

            // Create all definitely new entries.
            foreach (var p in DefinitelyNew) LuiObjectTableIndex[p.GetType()].Add(p, null);

            // Find old parameters with same name as new parameters using LINQ.
            var sameNames = (from p in OldParameters
                             join q in NewParameters on p.Name equals q.Name
                             select new { Old = p, New = q }).ToList(); // Same ToList() copy trick.

            foreach (var pair in sameNames)
                //if (!pair.Old.Equals(pair.New)) // Existing entry needs update.
                if (pair.Old.NeedsReinstantiation(pair.New))
                {
                    var luiObject = LuiObjectTableIndex[pair.Old.GetType()][pair.Old];
                    // if NeedsReinstantion, Dispose and set null.
                    // else if NeedsUpdate, update but don't null.
                    if (luiObject != null) luiObject.Dispose();
                    LuiObjectTableIndex[pair.Old.GetType()].Remove(pair.Old);
                    LuiObjectTableIndex[pair.New.GetType()].Add(pair.New, null);
                }
                else if (pair.Old.NeedsUpdate(pair.New))
                {
                    var luiObject = (LuiObject<P>)LuiObjectTableIndex[pair.Old.GetType()][pair.Old];
                    if (luiObject != null) luiObject.Update(pair.New);
                    LuiObjectTableIndex[pair.Old.GetType()].Remove(pair.Old);
                    LuiObjectTableIndex[pair.New.GetType()].Add(pair.New, luiObject);
                }

            // At this point, all entries in the object table are null 
            // EXCEPT those with no changed parameters OR which have been updated without reinstantiation.
        }

        /// <summary>
        ///     Disposes all LuiObjects in the LuiObject table.
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            if (disposing)
                foreach (var subtable in LuiObjectTableIndex.Values) // List of subtables
                    foreach (var kvp in subtable) // parameter/object pair
                        if (kvp.Value != null)
                            kvp.Value.Dispose();
        }

        /// <summary>
        ///     Serializes the LuiConfig instance to ConfigFile.
        /// </summary>
        public void Save()
        {
            Save(ConfigFile);
        }

        /// <summary>
        ///     Serializes the LuiConfig instance to a file.
        /// </summary>
        /// <param name="FileName"></param>
        public void Save(string FileName)
        {
            var serializer = new XmlSerializer(typeof(LuiConfig));
            try
            {
                using (var writer = new StreamWriter(FileName))
                {
                    serializer.Serialize(writer, this);
                }

                Saved = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        ///     Deserializes a LuiConfig instance from XML file.
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static LuiConfig FromFile(string FileName)
        {
            var serializer = new XmlSerializer(typeof(LuiConfig));
            LuiConfig Config = null;
            using (var reader = new StreamReader(FileName))
            {
                Config = (LuiConfig)serializer.Deserialize(reader);
            }

            return Config;
        }

        public void InstantiateConfiguration()
        {
            if (DryRun) return;
            // The topological sort ensures dependencies are resolved in a legal order.
            // Cyclic dependencies will result in exceptions.
            var dependencyOrderedParameters = LuiObjectParameters.TopologicalSort(p => p.Dependencies);
            foreach (var p in dependencyOrderedParameters)
                if (GetObject(p) == null)
                {
                    var dependencies = p.Dependencies.Select(d => GetObject(d));
                    SetObject(p, LuiObject.Create(p, dependencies));
                }
        }

        public async Task InstantiateConfigurationAsync()
        {
            await Task.Run(() => { InstantiateConfiguration(); });
        }

        public bool TryInstantiateConfiguration()
        {
            try
            {
                InstantiateConfiguration();
            }
            catch (Exception e)
            {
                Log.Error(e);
                return false;
            }

            return true;
        }

        public void InstantiateWithDependencies(LuiObjectParameters p)
        {
            if (GetObject(p) == null)
            {
                var ordered = p.Dependencies.TopologicalSort(q => q.Dependencies);
                foreach (var q in ordered) InstantiateWithDependencies(q);
                SetObject(p, LuiObject.Create(p, p.Dependencies.Select(d => GetObject(d))));
            }
        }

        public void OnParametersChanged(object sender, EventArgs e)
        {
            ParametersChanged.Raise(sender, e);
            if (!(sender is ParentForm)) Saved = false;
        }

        public static LuiConfig DummyConfig()
        {
            var config = new LuiConfig();
            var bf = new BeamFlagsParameters(typeof(DummyBeamFlags))
            {
                Name = "Dummy"
            };
            var cam = new CameraParameters(typeof(DummyAndorCamera))
            {
                Name = "Dummy"
            };
            var dg = new DelayGeneratorParameters(typeof(DummyDigitalDelayGenerator))
            {
                Name = "Dummy"
            };
            config.AddParameters(bf);
            config.AddParameters(cam);
            config.AddParameters(dg);
            return config;
        }

        #region Application parameters

        /* Application parameters have:
         *   Private fields
         *   Public getters and setters
         *   Public properties
         *   
         * The properties have no side effects and are used for serialization.
         * The Get/Set methods either trigger events indicating that the
         * configuration has changed or directly change the application state.
         */

        #region Dry run

        public bool DryRun { get; set; }

        #endregion

        #region ConfigFile

        public string ConfigFile { get; set; }

        public string GetConfigFile()
        {
            return ConfigFile;
        }

        public void SetConfigFile(string val)
        {
            ConfigFile = val;
        }

        #endregion

        #region LogFile

        public string LogFile { get; set; }

        public string GetLogFile()
        {
            return LogFile;
        }

        public void SetLogFile(string val)
        {
            LogFile = val;
        }

        #endregion

        #region LogLevel

        public string LogLevel { get; set; }

        public string GetLogLevel()
        {
            return LogLevel;
        }

        public void SetLogLevel(string val)
        {
            ((Hierarchy)LogManager.GetRepository()).Root.Level =
                ((Hierarchy)LogManager.GetRepository()).LevelMap[val];
            ((Hierarchy)LogManager.GetRepository()).RaiseConfigurationChanged(EventArgs.Empty);
            LogLevel = val;
        }

        #endregion

        #endregion
    }
}