﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Cosmos.VS.XSharp {
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = "XSharp")]
  [Name("XSharp")]
  [UserVisible(true)]
  [Order(Before = Priority.Default)]
  internal sealed class XSharpFormat : ClassificationFormatDefinition {
    public XSharpFormat() {
      this.DisplayName = "XSharp"; //human readable version of the name
      this.BackgroundColor = Colors.BlueViolet;
      this.TextDecorations = System.Windows.TextDecorations.Underline;
    }
  }

  // This class causes a classifier to be added to the set of classifiers
  [Export(typeof(IClassifierProvider))]
  [ContentType("XSharp")]
  internal class XSharpProvider : IClassifierProvider {
    // Import the classification registry to be used for getting a reference
    // to the custom classification type later.
    [Import]
    internal IClassificationTypeRegistryService ClassificationRegistry = null; // Set via MEF

    public IClassifier GetClassifier(ITextBuffer aBuffer) {
      return aBuffer.Properties.GetOrCreateSingletonProperty<XSharp>(delegate { 
        return new XSharp(ClassificationRegistry); 
      });
    }
  }

  // Classifier that classifies all text as an instance of the OrinaryClassifierType
  class XSharp : IClassifier {
    IClassificationType mClassificationType;

    internal XSharp(IClassificationTypeRegistryService aRegistry) {
      mClassificationType = aRegistry.GetClassificationType("XSharp");
    }

    // This method scans the given SnapshotSpan for potential matches for this classification.
    // In this instance, it classifies everything and returns each span as a new ClassificationSpan.
    // <param name="trackingSpan">The span currently being classified</param>
    // <returns>A list of ClassificationSpans that represent spans identified to be of this classification</returns>
    public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan aSpan) {
      //create a list to hold the results
      var xClassifications = new List<ClassificationSpan>();
      xClassifications.Add(new ClassificationSpan(new SnapshotSpan(aSpan.Snapshot, new Span(aSpan.Start, aSpan.Length)), mClassificationType));
      return xClassifications;
    }

    //#pragma warning disable 67
    // This event gets raised if a non-text change would affect the classification in some way,
    // for example typing /* would cause the classification to change in C# without directly affecting the span.
    public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
    //#pragma warning restore 67
  }
}
