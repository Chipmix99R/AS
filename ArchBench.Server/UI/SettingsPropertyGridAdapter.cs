using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchBench.PlugIns;

namespace ArchBench.Server.UI
{
    internal class SettingsPropertyGridAdapter : ICustomTypeDescriptor
    {
        readonly IArchBenchSettings mSettings;

        public SettingsPropertyGridAdapter( IArchBenchSettings aSettings )
        {
            mSettings = aSettings;
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName( this, true );
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent( this, true );
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName( this, true );
        }

        public EventDescriptorCollection GetEvents( Attribute[] attributes )
        {
            return TypeDescriptor.GetEvents( this, attributes, true );
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents( this, true );
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter( this, true );
        }

        public object GetPropertyOwner( PropertyDescriptor pd )
        {
            return mSettings;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes( this, true );
        }

        public object GetEditor( Type editorBaseType )
        {
            return TypeDescriptor.GetEditor( this, editorBaseType, true );
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ( (ICustomTypeDescriptor) this ).GetProperties( new Attribute[0] );
        }

        public PropertyDescriptorCollection GetProperties( Attribute[] attributes )
        {
            ArrayList properties = new ArrayList();
            foreach ( var key in mSettings )
            {
                properties.Add( new SettingsPropertyDescriptor( mSettings, key ) );
            }

            PropertyDescriptor[] props = (PropertyDescriptor[]) properties.ToArray(typeof(PropertyDescriptor));

            return new PropertyDescriptorCollection( props );
        }
    }
}
