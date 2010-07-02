



using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

using Mono.Unix.Native;


namespace Mango.Server {

	public class HttpResponse {

		private MemoryStream buffer = new MemoryStream ();

		public HttpResponse (HttpConnection connection, Encoding encoding)
		{
			Connection = connection;
			Encoding = encoding;

			Headers = new HttpHeaders ();
		}

		public HttpConnection Connection {
			get;
			private set;
		}

		public HttpHeaders Headers {
			get;
			private set;
		}

		public Encoding Encoding {
			get;
			private set;
		}

		public int StatusCode {
			get;
			set;
		}

		public void Write (string str)
		{
			byte [] data = Encoding.GetBytes (str);
			buffer.Write (data, 0, data.Length);
			// Connection.Write (data);
		}

		public void SendFile (string file)
		{
			byte [] data = File.ReadAllBytes (file);
			buffer.Write (data, 0, data.Length);
			// Connection.SendFile (file);
		}

		public void Finish ()
		{
			WriteStatusCode ();
			WriteHeaders ();

			Connection.Write (buffer.GetBuffer ());
			Connection.Finish ();
		}

		private void WriteStatusCode ()
		{
			string line = String.Format ("HTTP/1.0 {0} {1}", StatusCode, GetStatusDescription (StatusCode));
			byte [] data = Encoding.GetBytes (line);

			Connection.Write (data);
		}

		private void WriteHeaders ()
		{
			Headers.ContentLength = buffer.Length;

			MemoryStream stream = new MemoryStream ();
			Headers.Write (stream, Encoding);

			Connection.Write (stream.GetBuffer ());
		}

		private static string GetStatusDescription (int code)
		{
			switch (code){
			case 100: return "Continue";
			case 101: return "Switching Protocols";
			case 102: return "Processing";
			case 200: return "OK";
			case 201: return "Created";
			case 202: return "Accepted";
			case 203: return "Non-Authoritative Information";
			case 204: return "No Content";
			case 205: return "Reset Content";
			case 206: return "Partial Content";
			case 207: return "Multi-Status";
			case 300: return "Multiple Choices";
			case 301: return "Moved Permanently";
			case 302: return "Found";
			case 303: return "See Other";
			case 304: return "Not Modified";
			case 305: return "Use Proxy";
			case 307: return "Temporary Redirect";
			case 400: return "Bad Request";
			case 401: return "Unauthorized";
			case 402: return "Payment Required";
			case 403: return "Forbidden";
			case 404: return "Not Found";
			case 405: return "Method Not Allowed";
			case 406: return "Not Acceptable";
			case 407: return "Proxy Authentication Required";
			case 408: return "Request Timeout";
			case 409: return "Conflict";
			case 410: return "Gone";
			case 411: return "Length Required";
			case 412: return "Precondition Failed";
			case 413: return "Request Entity Too Large";
			case 414: return "Request-Uri Too Long";
			case 415: return "Unsupported Media Type";
			case 416: return "Requested Range Not Satisfiable";
			case 417: return "Expectation Failed";
			case 422: return "Unprocessable Entity";
			case 423: return "Locked";
			case 424: return "Failed Dependency";
			case 500: return "Internal Server Error";
			case 501: return "Not Implemented";
			case 502: return "Bad Gateway";
			case 503: return "Service Unavailable";
			case 504: return "Gateway Timeout";
			case 505: return "Http Version Not Supported";
			case 507: return "Insufficient Storage";
			}
			return "";
		}
	}

}



