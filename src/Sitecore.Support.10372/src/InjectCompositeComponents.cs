using Sitecore.Data.Items;
using Sitecore.Extensions.XElementExtensions;
using Sitecore.Mvc.Extensions;
using Sitecore.XA.Foundation.Presentation.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Sitecore.Support.XA.Feature.Composites.Pipelines.GetXmlBasedLayoutDefinition
{
  public class InjectCompositeComponents : Sitecore.XA.Feature.Composites.Pipelines.GetXmlBasedLayoutDefinition.InjectCompositeComponents
  {
    protected override void MergeComposites(XElement layoutXml, List<XElement> compositeRenderings, KeyValuePair<int, Item> composite, int dynamicPlaceholderId, string parentPh, string partialDesignId)
    {
      compositeRenderings.ForEach((Action<XElement>)(compositeRendering =>
      {
        string relativePlaceholder = this.GetRelativePlaceholder(compositeRendering, composite, dynamicPlaceholderId, parentPh);
        compositeRendering.Attribute((XName)"ph").SetValue((object)relativePlaceholder);
        this.SetAttribute(compositeRendering, "cmps-item", (object)composite.Value.ID);
        this.SetTrueAttribute(compositeRendering, "cmps");
        this.HandleEmptyDatasources(composite.Value, compositeRendering);
        this.SetPartialDesignId(compositeRendering, partialDesignId);
        this.SetOriginalDataSource(compositeRendering);
      }));
      foreach (XElement xelement in layoutXml.Descendants((XName)"d").GroupBy<XElement, string>((Func<XElement, string>)(element => element.GetAttributeValue("id"))).Select<IGrouping<string, XElement>, XElement>((Func<IGrouping<string, XElement>, XElement>)(elements => elements.First<XElement>())))
      {
        XElement device = xelement;
        DeviceModel deviceModel = new DeviceModel(device.ToXmlNode());
        if (deviceModel.DeviceId == Context.Device.ID)
        {
          compositeRenderings = compositeRenderings.Where<XElement>((Func<XElement, bool>)(e => this.NotInjectedIntoDevice(new Sitecore.XA.Foundation.Presentation.Layout.RenderingModel(e.ToXmlNode()), deviceModel))).ToList<XElement>();
          compositeRenderings.ForEach((Action<XElement>)(compositeRendering => device.Add((object)compositeRendering)));
        }
      }
    }
    private void SetAttribute(XElement composite, string attribute, object value)
    {
      if (composite.Attribute((XName)attribute) != null)
        return;
      XAttribute xattribute = new XAttribute((XName)attribute, value);
      composite.Add((object)xattribute);
    }
  }
}
