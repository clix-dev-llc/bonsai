﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Bonsai.Expressions
{
    /// <summary>
    /// Represents a dynamic assignment between a selected input source and a property of
    /// a workflow element.
    /// </summary>
    public sealed class PropertyMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMapping"/> class.
        /// </summary>
        public PropertyMapping()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMapping"/> class with
        /// the specified property name and source selector.
        /// </summary>
        /// <param name="name">
        /// The name of the property that will be assigned by this mapping.
        /// </param>
        /// <param name="selector">
        /// A string that will be used to select the input source that will assign
        /// values to this property mapping.
        /// </param>
        public PropertyMapping(string name, string selector)
        {
            Name = name;
            Selector = selector;
        }

        /// <summary>
        /// Gets or sets the name of the property that will be assigned by this mapping.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a string that will be used to select the input source that will assign
        /// values to this property mapping.
        /// </summary>
        [XmlAttribute("selector")]
        public string Selector { get; set; }
    }
}
