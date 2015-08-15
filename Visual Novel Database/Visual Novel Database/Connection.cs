using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Globalization;
using System.IO;

namespace Visual_Novel_Database
{
    //This class handles the connection to the API
    //meaning: Connection, Login, Send request and receive response
    //
    //CREDIT FOR THIS GOES TO: fredthebarber from the VNDB.org forum
    class Connection
    {
        private string LoginString; //Login string

        private TcpClient tcpClient;
        private Stream stream;

        private string jsonPayLoad; //Contains API response (in json format)

        private byte EndOfCommand = 0x04; //Has to be attached to end of command (EOF)

        #region properties

        public string jsonresponse
        {
            get
            {
                return jsonPayLoad;
            }
            set
            {
                jsonPayLoad = value;
            }
        }

        #endregion properties

        #region enums

        //Handles responsetype
        public enum ResponseType
        {
            Ok,
            Error,
            Unknown,
        }     

        #endregion enums

        #region methods

        private static Tuple<string, ResponseType>[] ResponseTypeMap = new Tuple<string, ResponseType>[]
        {
            new Tuple<string, ResponseType>("Ok", ResponseType.Ok),
            new Tuple<string, ResponseType>("Error", ResponseType.Error),
        };

        public Connection()
        {
        }

        //Opens connection
        public void Open()
        {
            tcpClient = new TcpClient();
            tcpClient.Connect ("api.vndb.org", 19534);
            stream = tcpClient.GetStream();
        }

        //Login
        public int Login(string loginName)
        {
            LoginString = "login {\"protocol\":1,\"client\":\"" + loginName + "\",\"clientver\":0.1}";

            ResponseType response = new ResponseType();

            response = IssueCommand(LoginString);

            if ((response == ResponseType.Error) || (response == ResponseType.Unknown))
                return 1;
            else
                return 0;
        }

        //Issue command
        public int Query(string query)
        {
            ResponseType response = new ResponseType();
            response = IssueCommand(query);

            if (response == ResponseType.Error)
                return 1;
            else
                return 0;
        }

        //Issue request and handle response
        public ResponseType IssueCommand(string command)
        {
            byte[] encoded = Encoding.UTF8.GetBytes(command); //encode request to UTF8 format
            byte[] requestBuffer = new byte[encoded.Length + 1]; 
            
            Buffer.BlockCopy(encoded, 0, requestBuffer, 0, encoded.Length);

            requestBuffer[encoded.Length] = EndOfCommand; //Attach EOF to request

            stream.Write(requestBuffer, 0, requestBuffer.Length);  //Send request

            byte[] responseBuffer = new byte[4096];

            int totalRead = 0;

            while (true) //Read all bytes
            {
                int currentRead = stream.Read(responseBuffer, totalRead, responseBuffer.Length - totalRead); //read from stream

                if (currentRead == 0)
                    throw new Exception("Connection closed while reading login response");

                totalRead += currentRead;

                if (IsCompleteMessage(responseBuffer, totalRead)) //Check if message is complete
                    break;

                if (totalRead == responseBuffer.Length) //Embiggen buffer if necessary
                {
                    byte[] biggerBuffer = new byte[responseBuffer.Length * 2];
                    Buffer.BlockCopy(responseBuffer, 0, biggerBuffer, 0, responseBuffer.Length);
                    responseBuffer = biggerBuffer;
                }
            }

            string Response = Encoding.UTF8.GetString(responseBuffer, 0, totalRead - 1); //Get in string format

            int firstspace = Response.IndexOf(' '); //Look for whitespace

            if (firstspace == -1) //Only got 'OK' back -> no jsonpayload (just logged in)
            {
                Tuple<string, ResponseType> responseTypeEntry = ResponseTypeMap.FirstOrDefault(l => string.Compare(l.Item1, Response, StringComparison.OrdinalIgnoreCase) == 0);

                if (responseTypeEntry != null)
                    return responseTypeEntry.Item2;

            }
            else //Got response + jsonpayload
            {
                string responseTypeString = Response.Substring(0, firstspace);

                Tuple<string, ResponseType> responseTypeEntry = ResponseTypeMap.FirstOrDefault(l => string.Compare(l.Item1, responseTypeString, StringComparison.OrdinalIgnoreCase) == 0);

                if (responseTypeEntry != null)
                {
                    jsonPayLoad = Response.Substring(firstspace + 1);
                    return responseTypeEntry.Item2;
                }
                else
                {
                    jsonPayLoad = Response.Substring(firstspace + 1);
                    return ResponseType.Unknown;
                }
            }
            return (ResponseType.Error);                    
        }

        //Check if message is complete
        public bool IsCompleteMessage(byte[] message, int bytesUsed)
        {

            if (bytesUsed == 0)
                throw new Exception("You have a bug, in IsCompleteMessage");

            if (message[bytesUsed - 1] == EndOfCommand) //Look for EOF byte
                return true;
            else
                return false;
        }

        //Close connection
        public void CloseConnection()
        {
            tcpClient.Close();
        }

        #endregion methods

    }


}

