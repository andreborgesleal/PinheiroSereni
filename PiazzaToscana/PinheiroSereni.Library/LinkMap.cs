using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace PinheiroSereni.Library
{
    public class LinkMap
    {
        public string linkText { get; set; }
        public string actionName { get; set; }
        public string controllerName { get; set; }
        public object routeValues { get; set; }
        public object htmlAtributes { get; set; }

        public LinkMap()
        {
            linkText = "";
            actionName = "";
            controllerName = "";
        }

        public LinkMap(string _linkText, string _actionName, string _controllerName)
        {
            linkText = _linkText;
            actionName = _actionName;
            controllerName = _controllerName;
        }

        public static ViewDataDictionary getLinkMap(ViewDataDictionary ViewData)
        {

            System.Web.HttpContext web = System.Web.HttpContext.Current; ;

            int contadorSegmentos = web.Request.Url.Segments.Count();
            int contadorParametros = web.Request.QueryString.Count ;
            contadorParametros = 0;
            string virtualPath = "";
            if (web.Request.Url.AbsolutePath.Contains("Error") && !web.Request.Url.AbsolutePath.Contains("AccessError"))
                virtualPath = "Error/";
            else
                for (int i = 1; i < contadorSegmentos - contadorParametros; i++)
                    virtualPath += web.Request.Url.Segments[i].ToString();



            string path = web.Request.PhysicalApplicationPath + "\\App_Data";
            XDocument documento = XDocument.Load(path + "\\SiteMap.xml");
            XElement element = (from c in documento.Descendants("WebForm")
                                where c.Attribute("url").Value.ToLower().Equals(virtualPath.ToLower())
                                select c).FirstOrDefault();

            if (element != null)
            {
                ViewData["Message"] = element.Element("Mensagem").Element("Message").Value;
                ViewData["InformativoTitulo"] = element.Element("Mensagem").Element("InformativoTitulo").Value;
                if (element.Element("Mensagem").Element("InformativoDetalhe1") != null)
                    ViewData["InformativoDetalhe1"] = element.Element("Mensagem").Element("InformativoDetalhe1").Value;
                if (element.Element("Mensagem").Element("InformativoDetalhe2") != null)
                    ViewData["InformativoDetalhe2"] = element.Element("Mensagem").Element("InformativoDetalhe2").Value;
                if (element.Element("Mensagem").Element("InformativoDetalhe3") != null)
                    ViewData["InformativoDetalhe3"] = element.Element("Mensagem").Element("InformativoDetalhe3").Value;
                if (element.Element("Mensagem").Element("InformativoDetalhe4") != null)
                    ViewData["InformativoDetalhe4"] = element.Element("Mensagem").Element("InformativoDetalhe4").Value;
                if (element.Element("Mensagem").Element("InformativoDetalhe5") != null)
                    ViewData["InformativoDetalhe5"] = element.Element("Mensagem").Element("InformativoDetalhe5").Value;

                ListLinkMap list = new ListLinkMap();

                foreach (XElement e in element.Descendants("Links").Elements())
                {
                    LinkMap lm = new LinkMap();
                    lm.linkText = e.Attribute("linkText").Value;
                    lm.actionName = e.Attribute("actionName").Value;
                    lm.controllerName = e.Attribute("controllerName").Value;

                    list.Add(lm);
                }

                if (list.Values.Count > 0 && ViewData["linkMap"] == null)
                    ViewData.Add("linkMap", list);

            }

            return ViewData;

        }

    }

    public class ListLinkMap : ILinkMap
    {
        private IList<LinkMap> elements; 

        public ListLinkMap()
        {
            elements = new List<LinkMap>();
        }

        public void Add(LinkMap linkMap)
        {
            elements.Add(linkMap);
        }

        public IList<LinkMap> Values
        {
            get { return elements; }
        }

    }

    public interface ILinkMap
    {
        void Add(LinkMap linkMap);
    }
}