namespace Sitecore.Support.XA.Feature.Composites.Pipelines.GetXmlBasedLayoutDefinition
{
  using Sitecore.Data.Items;
  using Sitecore.Extensions.XElementExtensions;
  using Sitecore.Mvc.Extensions;
  using Sitecore.XA.Feature.Composites;
  using Sitecore.XA.Foundation.Presentation.Layout;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml.Linq;

  public class InjectCompositeComponents : Sitecore.XA.Feature.Composites.Pipelines.GetXmlBasedLayoutDefinition.InjectCompositeComponents
  {
    protected override void MergeComposites(XElement layoutXml, List<XElement> compositeRenderings, KeyValuePair<int, Item> composite, int dynamicPlaceholderId, string parentPh, string partialDesignId)
    {
      compositeRenderings.ForEach(compositeRendering =>
      {
        var ph = GetRelativePlaceholder(compositeRendering, composite, dynamicPlaceholderId, parentPh);
        compositeRendering.Attribute("ph").SetValue(ph);
        SetAttribute(compositeRendering, Constants.CompositeItemXmlAttr, composite.Value.ID);
        SetTrueAttribute(compositeRendering, Constants.CompositeXmlAttr);
        HandleEmptyDatasources(composite.Value, compositeRendering);
        SetPartialDesignId(compositeRendering, partialDesignId);
        SetOriginalDataSource(compositeRendering);
      });

      var devices = layoutXml.Descendants("d").GroupBy(element => element.GetAttributeValue("id")).Select(elements => elements.First());
      foreach (var device in devices)
      {
        var deviceModel = new DeviceModel(device.ToXmlNode());
          compositeRenderings = compositeRenderings.Where(e => NotInjectedIntoDevice(new RenderingModel(e.ToXmlNode()), deviceModel)).ToList();
          compositeRenderings.ForEach(compositeRendering => device.Add(compositeRendering));
      }
    }
    private void SetAttribute(XElement composite, string attribute, object value)
    {
      if (composite.Attribute(attribute) == null)
      {
        var xattr = new XAttribute(attribute, value);
        composite.Add(xattr);
      }
    }
  }
}