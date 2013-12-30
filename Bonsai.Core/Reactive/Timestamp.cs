﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Reactive;

namespace Bonsai.Reactive
{
    [Combinator]
    [XmlType(Namespace = Constants.XmlNamespace)]
    [Description("Records the timestamp for each value produced by the sequence.")]
    public class Timestamp
    {
        public IObservable<Timestamped<TSource>> Process<TSource>(IObservable<TSource> source)
        {
            return source.Timestamp();
        }
    }
}