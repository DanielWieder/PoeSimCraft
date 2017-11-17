using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSimCraftImporter
{
    /* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */
    using System;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    [XmlRoot(ElementName = "Implicit")]
    public class Implicit
    {
        [XmlElement(ElementName = "string")]
        public List<string> String { get; set; }
    }

    [XmlRoot(ElementName = "Properties")]
    public class Properties
    {
        [XmlElement(ElementName = "string")]
        public List<string> String { get; set; }
    }

    [XmlRoot(ElementName = "ItemBase")]
    public class ItemBase
    {
        [XmlElement(ElementName = "Implicit")]
        public Implicit Implicit { get; set; }
        [XmlElement(ElementName = "Properties")]
        public Properties Properties { get; set; }
        [XmlAttribute(AttributeName = "ItemClass")]
        public string ItemClass { get; set; }
        [XmlAttribute(AttributeName = "Tags")]
        public string Tags { get; set; }
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "Level")]
        public string Level { get; set; }
        [XmlAttribute(AttributeName = "Strength")]
        public string Strength { get; set; }
        [XmlAttribute(AttributeName = "Dexterity")]
        public string Dexterity { get; set; }
        [XmlAttribute(AttributeName = "Intelligence")]
        public string Intelligence { get; set; }
        [XmlAttribute(AttributeName = "InventoryHeight")]
        public string InventoryHeight { get; set; }
        [XmlAttribute(AttributeName = "InventoryWidth")]
        public string InventoryWidth { get; set; }
        [XmlAttribute(AttributeName = "MetadataId")]
        public string MetadataId { get; set; }
        [XmlAttribute(AttributeName = "DropDisabled")]
        public string DropDisabled { get; set; }
    }

    [XmlRoot(ElementName = "ItemList")]
    public class ItemList
    {
        [XmlElement(ElementName = "ItemBase")]
        public List<ItemBase> ItemBase { get; set; }
        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
    }
}
