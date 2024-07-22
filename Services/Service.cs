using System.Xml.Serialization;
using System.Xml;
using TransformarXmlEmCSharpESalvarNoBanco.Models;

namespace TransformarXmlEmCSharpESalvarNoBanco.Services
{
    public class Service
    {
        public Evt Desserializar(string arquivo)
        {
            string xmlContent;
            using (StreamReader stream = new StreamReader(arquivo))
            {
                xmlContent = stream.ReadToEnd();
            }

            // Carregar o XML em um XmlDocument para obter o namespace
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            // Desserializar o XML para o objeto
            Evt resultado;
            using (StringReader stringReader = new StringReader(xmlContent))
            {
                XmlSerializer serializador = new XmlSerializer(typeof(Evt));
                resultado = (Evt)serializador.Deserialize(stringReader);
            }

            // Método para atribuir o Namespace para o objeto
            AssignNamespaces(xmlDoc.DocumentElement, resultado);

            return resultado;
        }

        public void AssignNamespaces(XmlNode node, object obj)
        {
            if (node == null || obj == null)
                return;

            // Tenta encontrar uma propriedade chamada "Namespace" no objeto
            var objType = obj.GetType();
            var namespaceProperty = objType.GetProperty("Namespace");
            if (namespaceProperty != null && namespaceProperty.CanWrite)
            {
                // Se a propriedade "Namespace" existir e puder ser escrita, atribui o namespace do nó XML a essa propriedade
                namespaceProperty.SetValue(obj, node.NamespaceURI);
            }

            // Itera sobre todos os filhos do nó XML
            foreach (XmlNode childNode in node.ChildNodes)
            {
                // Encontra a propriedade do objeto C# correspondente ao nome do nó XML filho
                var property = objType.GetProperties().FirstOrDefault(p =>
                    (p.GetCustomAttributes(typeof(XmlElementAttribute), false).FirstOrDefault() as XmlElementAttribute)?.ElementName == childNode.Name);

                if (property != null)
                {
                    // Se a propriedade correspondente for encontrada
                    var childObject = property.GetValue(obj);
                    if (childObject == null)
                    {
                        // Se o objeto filho for nulo, cria uma instância desse tipo
                        if (property.PropertyType == typeof(string))
                        {
                            // Se a propriedade é uma string, define diretamente o valor do nó XML
                            property.SetValue(obj, childNode.InnerText);
                        }
                        else
                        {
                            childObject = Activator.CreateInstance(property.PropertyType);
                            property.SetValue(obj, childObject);
                            // Recursivamente chama AssignNamespaces para o nó filho e o objeto filho
                            AssignNamespaces(childNode, childObject);
                        }
                    }
                    else
                    {
                        // Se o objeto filho não for nulo, chama recursivamente AssignNamespaces
                        AssignNamespaces(childNode, childObject);
                    }
                }
            }
        }
    }
}