using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Enumeracoes;
using PinheiroSereni.Dominio.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;     

namespace PinheiroSereni.Dominio.Control
{
    public class Torpedo : ITorpedo
    {
        public Validate send(string mensagem, string nome, string telefoneCliente, string telefoneCorretor)
        {
            Validate result = new Validate() { Message = MensagemPadrao.Message(0).ToString() };
            try
            {

                System.Net.HttpWebRequest wRequest;
                Uri targetURI = new Uri("http://webapi.comtele.com.br/api/api_fuse_connection.php?fuse=get_id&user=60705&pwd=multisul");
                wRequest = (System.Net.HttpWebRequest)HttpWebRequest.Create(targetURI);

                System.IO.StreamReader strReader = new System.IO.StreamReader(wRequest.GetResponse().GetResponseStream());

                string id = strReader.ReadToEnd().Replace("\n", "");

                strReader.Close();

                // Enviar mensagem
                targetURI = new Uri("http://webapi.comtele.com.br/api/api_fuse_connection.php?fuse=send_msg&id=" + id + "&from=" + telefoneCliente + "&msg=Corretor, você deve ligar para o cliente " + nome + " de nº " + telefoneCliente + "&number=" + telefoneCorretor);
                wRequest = (System.Net.HttpWebRequest)HttpWebRequest.Create(targetURI);

                strReader = new System.IO.StreamReader(wRequest.GetResponse().GetResponseStream());

                string retornoTorpedo = strReader.ReadToEnd().Replace("\n", "");

                strReader.Close();

                //if (retornoTorpedo != "")
                //    throw new PinheiroSereniException(new Validate() { Message = retornoTorpedo });

                //User: 50019
                //Senha: apiteste2

                // recuperar o ID
                //http://webapi.comtele.com.br/api/api_fuse_connection.php?fuse=get_id&user=USUÁRIO&pwd=SENHA 


                // Enviar a mensagem
                //http://webapi.comtele.com.br/api/api_fuse_connection.php?fuse=send_msg&id=ID&from=ORIGEM&msg=MENSAGEM&number=DESTINO


                //3) Agendar envio de mensagem

                //http://webapi.comtele.com.br/_api/api_fuse_connection.php?fuse=send_msg&id="CODIGO_DO_ID"&from=FROM&msg=MENSAGEM&number=TELEFONES_SEPARADO_POR_VIRGULAS&sch_date=YYYY-MM-DD&sch_hour=HH:MM


                //Parâmetros:
                //- ID => proveniente da chamada get_id
                //- FROM => Campo DE
                //- MSG => Mensagem texto (lembre-se que todos os caracteres deverão estar no padrão URL_ENCODE)
                //- NUMBER => Telefones separados por vírgula lembrando que a limitação é só da URL
                // - SCH_DATE => Data de agendamento no seguinte formato (YYYY-MM-DD)
                //- SCH_HOUR => Hora do agendamento no formato (HH:MM)

                //Todos os campos são obrogatórios na API, porém caso não seja especificado os parâmetros sch_date e sch_hour será assumido de forma padrão como mensagem instantânea.


                //ConsultarSaldo:

                ////http://webapi.comtele.com.br/api/api_fuse_connection.php?fuse=get_balance&id=ID


                //VisualizarRelatório

                //http://webapi.comtele.com.br/api/api_fuse_connection.php?fuse=get_report&id=ID&dt_begin=DATA1&dt_end=DATA2&mode=MODE

                //ID: ID fornecidopeloprimeiropasso.

                //DATA1: Data inicial do relatório. Formato YYYY-MM-DD

                //DATA2: Data final do relatório. Formato YYYY-MM-DD

                //MODE:resume :RelatórioResumido

                //MODE:detail :RelatórioDetalhado


                //Segue a lista de possiveis erros:


                //ERR_00 : Disabled user

                //ERR_01 : Report error

                //ERR_02 : Balance error

                //ERR_03 : Credits expired

                //ERR_11 : ID Error

                //FAIL_ON_CREDITS

                //FAIL_CHECK_NUMBER

                //FAIL_ON_SEND
            }
            catch (PinheiroSereniException ex)
            {
                PinheiroSereniException.saveError(ex, GetType().FullName);
                result = new Validate() { Code = 15, Message = MensagemPadrao.Message(15).ToString(), MessageBase = ex.Message };
            }

            return result;
        }
    }
}
