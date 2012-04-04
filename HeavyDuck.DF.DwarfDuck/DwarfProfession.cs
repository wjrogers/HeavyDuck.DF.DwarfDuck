using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace HeavyDuck.DF.DwarfDuck
{
    internal class DwarfProfession : IEquatable<DwarfProfession>
    {
        private readonly dfproto.ProfessionAttr m_attr;
        private readonly Lazy<Image> m_image;
        private readonly Lazy<DwarfProfession> m_parent;

        public DwarfProfession(dfproto.ProfessionAttr attr)
        {
            m_attr = attr;
            m_image = new Lazy<Image>(() => DwarfGraphics.GetImage(m_attr.Key));
            m_parent = new Lazy<DwarfProfession>(() => GameData.GetProfession(m_attr.Parent));
        }

        public int ID
        {
            get { return m_attr.Id; }
        }

        public string Key
        {
            get { return m_attr.Key; }
        }

        public string Caption
        {
            get { return m_attr.Caption; }
        }

        public bool CanAssignLabor
        {
            get { return m_attr.CanAssignLabor; }
        }

        public bool IsMilitary
        {
            get { return m_attr.Military; }
        }

        public string Category
        {
            get
            {
                if (this.HasParent)
                    return this.Parent.Category;
                else
                    return this.Caption;
            }
        }

        public Image Image
        {
            get { return m_image.Value; }
        }

        public DwarfProfession Parent
        {
            get { return m_parent.Value; }
        }

        public bool HasParent
        {
            get { return m_parent.Value.ID != GameData.NONE; }
        }

        public bool Equals(DwarfProfession other)
        {
            return other != null
                && other.ID == this.ID;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DwarfProfession);
        }

        public override int GetHashCode()
        {
            return this.ID;
        }

        public override string ToString()
        {
            return this.Caption;
        }
    }
}
