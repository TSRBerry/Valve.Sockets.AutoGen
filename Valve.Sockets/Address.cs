/*
 *  Managed C# wrapper for GameNetworkingSockets library by Valve Software
 *  Copyright (c) 2018 Stanislav Denisov
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in all
 *  copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *  SOFTWARE.
 */

using System.Runtime.InteropServices;

namespace Valve.Sockets {
	[StructLayout(LayoutKind.Sequential)]
	public struct Address : IEquatable<Address> {
        public Array16<byte> IP;
		public ushort Port;

        public bool IsLocalHost => Native.SteamAPI_SteamNetworkingIPAddr_IsLocalHost(ref this);

		public string GetIP()
        {
			return IP.ParseIP();
		}

		public void SetLocalHost(ushort port) {
			Native.SteamAPI_SteamNetworkingIPAddr_SetIPv6LocalHost(ref this, port);
		}

		public void SetAddress(string ip, ushort port) {
			if (!ip.Contains(":"))
				Native.SteamAPI_SteamNetworkingIPAddr_SetIPv4(ref this, ip.ParseIPv4(), port);
			else
				Native.SteamAPI_SteamNetworkingIPAddr_SetIPv6(ref this, ip.ParseIPv6(), port);
		}

		public bool Equals(Address other) {
            return Native.SteamAPI_SteamNetworkingIPAddr_IsEqualTo(ref this, ref other);
        }
	}
}