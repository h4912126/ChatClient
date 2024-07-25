using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;
using System.IO;

public class ClientNet : MonoBehaviour
{
    private void Awake()
    {
        m_socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
        m_readOffset = 0;
        m_recvOffset = 0;
        // 16KB
        m_recvBuf = new byte[0x4000];
    }

    private void Update()
    {
        if (null == m_socket) return;
        if (m_connectState == ConnectState.Ing && m_connectAsync.IsCompleted)
        {
            // ���ӷ�����ʧ��
            if (!m_socket.Connected)
            {
                m_connectState = ConnectState.None;
                if (null != m_connectCb)
                    m_connectCb(false);
            }
        }

        if (m_connectState == ConnectState.Ok)
        {
            TryRecvMsg();
        }
    }

    private void TryRecvMsg()
    {
        // ��ʼ������Ϣ
        m_socket.BeginReceive(m_recvBuf, m_recvOffset, m_recvBuf.Length - m_recvOffset, SocketFlags.None, (result) =>
        {
            // �������Ϣ�����������ص�

            // ���len�Ƕ�ȡ���ĳ��ȣ�����һ����һ����������Ϣ�ĳ��ȣ�����������Ҫ����ͷ�������ֽ���Ϊ��ʵ����Ϣ����
            var len = m_socket.EndReceive(result);

            if (len > 0)
            {
                m_recvOffset += len;
                m_readOffset = 0;

                if (m_recvOffset - m_readOffset >= 2)
                {
                    // ͷ�����ֽ�����ʵ��Ϣ���ȣ�ע���ֽ�˳���Ǵ��
                    int msgLen = m_recvBuf[m_readOffset + 1] | (m_recvBuf[m_readOffset] << 8);

                    if (m_recvOffset >= (m_readOffset + 2 + msgLen))
                    {
                        // ������Ϣ
                        string msg = System.Text.Encoding.UTF8.GetString(m_recvBuf, m_readOffset + 2, msgLen);
                        Debug.Log("Recv msgLen: " + msgLen + ", msg: " + msg);
                        if (null != m_recvMsgCb)
                            m_recvMsgCb(msg);

                        m_readOffset += 2 + msgLen;
                    }
                }

                // buf��λ
                if (m_readOffset > 0)
                {
                    for (int i = m_readOffset; i < m_recvOffset; ++i)
                    {
                        m_recvBuf[i - m_readOffset] = m_recvBuf[i];
                    }
                    m_recvOffset -= m_readOffset;
                }
            }
        }, this);
    }

    /// <summary>
    /// ���ӷ����
    /// </summary>
    /// <param name="host">IP��ַ</param>
    /// <param name="port">�˿�</param>
    /// <param name="cb">�ص�</param>
    public void Connect(string host, int port, Action<bool> cb)
    {
        m_connectCb = cb;
        m_connectState = ConnectState.Ing;
        m_socket.SendTimeout = 100;
        m_connectAsync = m_socket.BeginConnect(host, port, (IAsyncResult result) =>
        {
            // ���ӳɹ�������������ʧ�ܲ����������
            var socket = result.AsyncState as Socket;
            socket.EndConnect(result);
            m_connectState = ConnectState.Ok;
            m_networkStream = new NetworkStream(m_socket);
            Debug.Log("Connect Ok");
            if (null != m_connectCb) m_connectCb(true);
        }, m_socket);

        Debug.Log("BeginConnect, Host: " + host + ", Port: " + port);
    }
 
/// <summary>
/// ע����Ϣ���ջص�����
/// </summary>
/// <param name="cb">�ص�����</param>
public void RegistRecvMsgCb(Action<string> cb)
    {
        m_recvMsgCb = cb;
    }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="bytes">��Ϣ���ֽ���</param>
    public void SendData(string message)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(message + "\n");
        m_networkStream.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// �ر�Sockete
    /// </summary>
    public void CloseSocket()
    {
        m_socket.Shutdown(SocketShutdown.Both);
        m_socket.Close();
    }

    /// <summary>
    /// �ж�Socket�Ƿ�����״̬
    /// </summary>
    /// <returns></returns>
    public bool IsConnected()
    {
        return m_socket.Connected;
    }

    private enum ConnectState
    {
        None,
        Ing,
        Ok,
    }

    private Action<bool> m_connectCb;
    private Action<string> m_recvMsgCb;
    private ConnectState m_connectState = ConnectState.None;
    private IAsyncResult m_connectAsync;

    private byte[] m_recvBuf;
    private int m_readOffset;
    private int m_recvOffset;
    private Socket m_socket;
    private NetworkStream m_networkStream;

    private static ClientNet s_instance;
    public static ClientNet instance
    {
        get
        {
            if (null == s_instance)
            {
                var go = new GameObject("ClientNet");
                s_instance = go.AddComponent<ClientNet>();
            }

            return s_instance;
        }
    }

}

