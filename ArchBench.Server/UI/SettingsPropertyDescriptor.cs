using System;
using System.ComponentModel;
using ArchBench.PlugIns;

namespace ArchBench.Server.UI
{
    class SettingsPropertyDescriptor : PropertyDescriptor
    {

        internal SettingsPropertyDescriptor( IArchBenchSettings aSettings, string key ) : base( key.ToString(), null )
        {
            Settings = aSettings;
            Key = key;
        }

        public IArchBenchSettings Settings { get; }
        public string             Key      { get; }

        public override Type PropertyType => Settings[Key].GetType();

        public override void SetValue( object component, object value )
        {
            Settings[Key] = value.ToString();
        }

        public override object GetValue( object component )
        {
            return Settings[Key];
        }

        public override bool IsReadOnly => false;

        public override Type ComponentType => null;

        public override bool CanResetValue( object component )
        {
            return false;
        }

        public override void ResetValue( object component )
        {
        }

        public override bool ShouldSerializeValue( object component )
        {
            return false;
        }
    }
}
