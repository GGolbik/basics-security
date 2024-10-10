
namespace GGolbik.SecurityToolsTest;

/// <summary>
/// cat ./test-code-ca-0.crl | hexdump --no-squeezing --format '/1 "%02x"'
/// </summary>
public class TestData
{
    public static readonly string TestRootCaPem = @"Certificate:
    Data:
        Version: 3 (0x2)
        Serial Number:
            2f:75:44:67:23:b1:5a:4b:13:e3:d4:81:b2:b8:f4:f3:f7:96:56:a8
        Signature Algorithm: sha256WithRSAEncryption
        Issuer: CN=Test Root CA
        Validity
            Not Before: Aug 20 06:08:10 2024 GMT
            Not After : Dec 31 22:59:59 9999 GMT
        Subject: CN=Test Root CA
        Subject Public Key Info:
            Public Key Algorithm: rsaEncryption
                Public-Key: (2048 bit)
                Modulus:
                    00:b0:73:cd:79:4b:86:e6:38:70:4b:71:0d:1e:75:
                    30:b3:d5:d9:bf:bf:64:cb:27:05:74:b7:45:27:7b:
                    cd:b6:70:1a:03:d8:8e:22:02:88:08:a1:b5:65:c8:
                    53:90:d6:ec:42:9c:65:28:f7:27:1b:9e:1d:6e:85:
                    80:b9:d5:5a:0e:df:d5:87:91:31:19:55:18:56:3a:
                    48:94:4f:60:94:d4:c2:2e:1e:8f:d5:13:da:c5:aa:
                    dc:8f:8f:fa:42:21:f8:1d:47:18:dc:35:a6:c5:c4:
                    05:17:85:50:cc:6a:71:bf:5b:5a:27:15:e6:50:4f:
                    a4:a2:be:68:16:bc:81:54:80:4d:77:1c:ba:78:ed:
                    99:16:cc:e0:2d:b7:39:a4:7d:9d:f3:a9:08:6a:77:
                    4c:da:72:8f:11:b8:ca:a8:16:0b:ba:f1:fb:fa:5a:
                    bd:76:6c:a0:d3:0c:e0:ff:69:4c:4e:25:0a:5e:d0:
                    d6:ff:7c:86:07:ed:6e:46:1e:c1:8b:48:86:bb:1b:
                    45:3d:c0:0a:c4:50:73:18:3e:67:a7:a9:7b:84:1d:
                    1b:4f:33:c0:26:a4:c5:8b:9d:ff:27:89:24:3c:6d:
                    cd:a0:71:6a:7e:11:01:78:24:d8:c5:3b:fe:42:98:
                    00:04:fc:0f:75:b5:17:17:2d:02:77:81:fb:c5:93:
                    e7:5f
                Exponent: 65537 (0x10001)
        X509v3 extensions:
            X509v3 Basic Constraints: critical
                CA:TRUE
            X509v3 Key Usage: critical
                Digital Signature, Certificate Sign, CRL Sign
            X509v3 Subject Key Identifier: 
                85:FC:C9:21:1E:50:3A:43:A7:CD:36:37:44:92:E0:88:8A:1B:29:39
            X509v3 Authority Key Identifier: 
                85:FC:C9:21:1E:50:3A:43:A7:CD:36:37:44:92:E0:88:8A:1B:29:39
    Signature Algorithm: sha256WithRSAEncryption
    Signature Value:
        03:39:b2:35:4e:7b:a2:ba:53:d0:90:9c:81:a5:8c:8b:dd:cc:
        20:10:e7:ee:77:fb:40:7e:54:bd:0b:ba:ec:8f:71:1e:db:cb:
        8e:c6:82:3f:03:f9:d0:9f:b4:93:d3:9f:8a:2a:2a:32:79:ee:
        dc:5e:09:9e:85:40:d7:e3:c2:9d:c2:2a:f1:e1:c2:54:42:90:
        7a:cd:30:a0:c9:63:56:41:61:78:7a:04:02:95:f1:d5:b5:72:
        8c:62:48:ba:83:05:16:d4:25:d6:d6:e2:3e:c2:52:22:c4:07:
        3f:06:41:62:6e:45:a8:c7:bd:bf:4b:a3:ea:a4:65:44:ec:30:
        89:a7:1b:ca:eb:f0:82:41:1f:17:80:b9:9d:7d:e8:dc:94:c8:
        ca:9f:ef:31:a3:31:7f:da:6b:be:ad:6d:27:fd:64:35:70:0d:
        11:b0:ce:25:96:b2:b5:04:dd:06:42:50:27:aa:05:50:e1:6d:
        18:bc:53:5c:89:b6:3f:dd:94:59:91:99:7c:ab:61:d0:7f:69:
        02:59:e3:ab:9b:db:9e:e0:81:bc:18:ec:2f:83:44:65:d6:6b:
        6a:e6:fe:29:89:3e:f5:fa:fe:a2:7c:63:3c:12:87:99:02:69:
        0e:f4:b5:50:23:2e:4e:6f:71:30:93:25:be:d3:78:ca:d8:b3:
        78:ea:0f:76
-----BEGIN CERTIFICATE-----
MIIDITCCAgmgAwIBAgIUL3VEZyOxWksT49SBsrj08/eWVqgwDQYJKoZIhvcNAQEL
BQAwFzEVMBMGA1UEAwwMVGVzdCBSb290IENBMCAXDTI0MDgyMDA2MDgxMFoYDzk5
OTkxMjMxMjI1OTU5WjAXMRUwEwYDVQQDDAxUZXN0IFJvb3QgQ0EwggEiMA0GCSqG
SIb3DQEBAQUAA4IBDwAwggEKAoIBAQCwc815S4bmOHBLcQ0edTCz1dm/v2TLJwV0
t0Une822cBoD2I4iAogIobVlyFOQ1uxCnGUo9ycbnh1uhYC51VoO39WHkTEZVRhW
OkiUT2CU1MIuHo/VE9rFqtyPj/pCIfgdRxjcNabFxAUXhVDManG/W1onFeZQT6Si
vmgWvIFUgE13HLp47ZkWzOAttzmkfZ3zqQhqd0zaco8RuMqoFgu68fv6Wr12bKDT
DOD/aUxOJQpe0Nb/fIYH7W5GHsGLSIa7G0U9wArEUHMYPmenqXuEHRtPM8AmpMWL
nf8niSQ8bc2gcWp+EQF4JNjFO/5CmAAE/A91tRcXLQJ3gfvFk+dfAgMBAAGjYzBh
MA8GA1UdEwEB/wQFMAMBAf8wDgYDVR0PAQH/BAQDAgGGMB0GA1UdDgQWBBSF/Mkh
HlA6Q6fNNjdEkuCIihspOTAfBgNVHSMEGDAWgBSF/MkhHlA6Q6fNNjdEkuCIihsp
OTANBgkqhkiG9w0BAQsFAAOCAQEAAzmyNU57orpT0JCcgaWMi93MIBDn7nf7QH5U
vQu67I9xHtvLjsaCPwP50J+0k9OfiioqMnnu3F4JnoVA1+PCncIq8eHCVEKQes0w
oMljVkFheHoEApXx1bVyjGJIuoMFFtQl1tbiPsJSIsQHPwZBYm5FqMe9v0uj6qRl
ROwwiacbyuvwgkEfF4C5nX3o3JTIyp/vMaMxf9prvq1tJ/1kNXANEbDOJZaytQTd
BkJQJ6oFUOFtGLxTXIm2P92UWZGZfKth0H9pAlnjq5vbnuCBvBjsL4NEZdZraub+
KYk+9fr+onxjPBKHmQJpDvS1UCMuTm9xMJMlvtN4ytizeOoPdg==
-----END CERTIFICATE-----
";

    public static readonly string TestRootCaKeyPem = @"-----BEGIN PRIVATE KEY-----
MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQCwc815S4bmOHBL
cQ0edTCz1dm/v2TLJwV0t0Une822cBoD2I4iAogIobVlyFOQ1uxCnGUo9ycbnh1u
hYC51VoO39WHkTEZVRhWOkiUT2CU1MIuHo/VE9rFqtyPj/pCIfgdRxjcNabFxAUX
hVDManG/W1onFeZQT6SivmgWvIFUgE13HLp47ZkWzOAttzmkfZ3zqQhqd0zaco8R
uMqoFgu68fv6Wr12bKDTDOD/aUxOJQpe0Nb/fIYH7W5GHsGLSIa7G0U9wArEUHMY
PmenqXuEHRtPM8AmpMWLnf8niSQ8bc2gcWp+EQF4JNjFO/5CmAAE/A91tRcXLQJ3
gfvFk+dfAgMBAAECggEAFjTfZppT5gmzTuNG6I7iFrfdTsLvv0YawIIcxhJ6z1t+
AqmQgFo7iYPkbY9Pvlo2DfoFxyk3oZj9Zj6PS+k8YMPYaSvOIecyMrk6D3mKk6B6
YaMqAdcdAM8h2+rIRDtnMN6/k7MoLm/Jc5jXvtTPROsNG6nSEZ28VVj2MOpUsnAP
uF5nKCLhxc9JrWKKRQlUUSMPC6dzwOorZG1MXhlONxWnfuN0/EPZjyHq8uGQfZ5N
qv9/y/fb5BEHqm68MbLl+uzjD8km3N9cgSOFEOhZj9aUKyDa/Low6pxQ+zvS6+p1
aAuHV8stNDaNEG+8YaObP2MmyqOEElhwwrm0q7tFRQKBgQDQJ9GsLncrZCyhFtNA
SYN8xA0PAwPOn/ptu3eBmQR02chfmgcpV2dM/gVzwv9apseFehZVjQlCXYnKYyGJ
GBeWLE7BCVxhQSORNbatDHBCsH5uF0pzjdR2oC+7o+tq0c69inTA41V2SAKTwozH
V9GeSlsc1wdaUiULpH4+cGXUdQKBgQDZAoVbCEBPhGsNVvpqS/jZ7DfilD1daMvq
B1F9GRgLxMLgUXCZKcffIh5wbxwNQGrKHYoLpEHIdfPD5Mbk5C26EJ8Q8XLAqUH6
EFeaRY4JFr3Fk8Dc5GMFyJ/MWcw/APRjTMRE5TQeKehtDOxIfR3PbJ3ej1tYLaap
FVRNmKOCAwKBgQDJgGJ2vZxbAuQ5JPFnYELK+rZxe8eptDAnHbz4Vfp8a36PHXol
SRkU7Sq1/2RWivDGg4MvWhJGjmoe44vJPOtIUqgCMl+dPgOCRG8MYegihtw9Eore
BcRQ+Yx7ppj0lRn/XhLbzrYihF8KTuEc8CRZiT1eU8IoazC1bo1PQFszKQKBgBMo
r9FbpyHeFP12gmFEF9JVkpGEeO54RBiDUOR0hLT1SCc5yXEcSTMf9gQDKzzYRRVX
CksA03X5Q+41koG+y3Kz6Pc7+d+ckeCb9MAACAwxX8vDwbE+0KdAESufefLOCMWD
j4htm+5V0Nlf3LSBp1Iays1NZskgLqia43h+U2E1AoGBALGmJBD8pBaZlao/ooZS
qg5hH4YUKGi6zdsJLed3aAXq6nQFPWakR8bGIxCKJ1NtCFz9F2XpiRr+ZGPns0vH
1H6hsIjMfodJSBOk49kjzBOb4iQ+qrBN122RccKngF8auAe+wO30mBZZpUajANyl
JpVFvk2tFJUux6FXOAK+8WGw
-----END PRIVATE KEY-----
";
    public static readonly byte[] TestRootCaCrl0Der = Convert.FromHexString("30820171305b020101300d06092a864886f70d01010b050030173115301306035504030c0c5465737420526f6f74204341170d3234303832303036303831325a180f39393939313233313232353935395aa00e300c300a0603551d140403020100300d06092a864886f70d01010b05000382010100250ab42bb8a377d6540999288cc8b05374947ae8e84aa5f83295dc441a034bbb1ea076cec18a34f785f1d0bda158039c000865d660728ab252bd3cdad110f0bd85ef3dab15aa2bc19cc53dd0bc1bb1fbbe7d3d41f79cdb41a5ed58f8d76675a7cf2647eadf8cdb6412898d0f14b53d0ef7f73ba7a61a2534cb31166afa4dcf7103dfc0a55e2295de18fd11cae7f5c9158b58b4b848a8d98fc7bc63e89d0155a162d9b9469d4907140dc0b9e15f27275ee67471e99373fc83636e23904ac541d0790f17a2ba6c32319fed830364c8b59b109d0dd58f84a4129877d63fcfe7277b2f65dffba419dcce3b6eda5245d963840402f8f5e6ff28cf15004122dcd9daa6");
    public static readonly byte[] TestRootCaCrl2Der = Convert.FromHexString("308201c23081ab020101300d06092a864886f70d01010b050030173115301306035504030c0c5465737420526f6f74204341170d3234303832303036303831325a180f39393939313233313232353935395a304e30250214136cc738b7446543aaa4f11c8649226687f09cb3170d3234303832303036303831325a302502142756129dbaab9acfbc53b7594fc4b6644f402b8f170d3234303832303036303831325aa00e300c300a0603551d140403020101300d06092a864886f70d01010b050003820101001b1867d3a2643d89dde71fb193f69df899e3070ede625a5a4b61bf36ce67ec28c7d0deb2693a6dd928e0b41eacbd3aac36555614617267030caac44c076acbb5de81c8cc8768bc940754ab06b02239ffa43b7755db7d0cc01fd98df5b90de2898e2ce413f93aac2eb540225a2fa02441b1b0d2abb871f02712bffa830f66b52fc1ea7354de908546398afe674d73e5400bdada028f3ca93e983009081a7b3a828fffae42a3a62cdbe203e764108df1fcf7eebd7d577891c74ee7ca7785477f88817cec0abfa42fded80a95d95febf836bdb5ec3c26059d84d4fc84cd47ece9da33851721488ae5dacfd62054ae65749effb30a6363f38a5483bf19a5c3004692");

    public static readonly string TestCodeCaPem = @"Certificate:
    Data:
        Version: 3 (0x2)
        Serial Number:
            27:56:12:9d:ba:ab:9a:cf:bc:53:b7:59:4f:c4:b6:64:4f:40:2b:8f
        Signature Algorithm: sha256WithRSAEncryption
        Issuer: CN=Test Root CA
        Validity
            Not Before: Aug 20 06:08:11 2024 GMT
            Not After : Dec 31 22:59:59 9999 GMT
        Subject: CN=Test CodeSigning CA
        Subject Public Key Info:
            Public Key Algorithm: rsaEncryption
                Public-Key: (2048 bit)
                Modulus:
                    00:8b:e3:45:6a:51:64:75:22:a6:5a:70:d1:0a:e1:
                    7a:14:95:c2:e6:06:8a:78:27:37:0a:09:f7:ca:d6:
                    d0:b2:0c:29:31:6c:a3:5f:35:d4:d2:2f:ba:b5:cd:
                    b3:be:f1:19:ec:a3:7c:5a:6e:a9:2a:9e:c5:88:99:
                    4d:47:55:ce:82:4c:66:5b:b6:c9:1d:01:33:dc:8a:
                    18:f1:b9:a5:a8:77:02:0d:2f:f7:6d:39:99:c5:d6:
                    3a:46:7e:6d:35:eb:29:25:9e:05:8b:b7:33:ad:aa:
                    f5:37:6b:57:3d:cf:b7:ab:c1:49:6f:fc:ff:9d:3d:
                    8e:97:5c:df:f9:4c:8a:39:e8:71:f7:3f:48:33:47:
                    24:95:2a:0b:b2:1c:22:fe:4c:83:7f:2d:13:e1:92:
                    84:26:17:d5:17:75:21:d7:60:09:0a:56:9a:a5:2b:
                    54:0d:ea:ef:a5:0f:1b:62:5b:50:ce:6c:8e:f6:2f:
                    ad:52:6f:3e:49:88:f3:dc:a3:d4:38:1e:b9:1a:41:
                    d9:e9:76:ab:9b:94:71:36:18:01:93:b0:8f:1a:35:
                    d9:c3:e6:53:cb:90:3d:29:5b:c2:70:2b:d6:cc:0b:
                    78:6a:73:20:32:a4:57:dd:7e:d7:c0:be:60:97:d1:
                    de:eb:84:70:93:68:75:5d:fa:dc:51:67:48:e1:a6:
                    2d:5d
                Exponent: 65537 (0x10001)
        X509v3 extensions:
            X509v3 Basic Constraints: critical
                CA:TRUE
            X509v3 Key Usage: critical
                Digital Signature, Certificate Sign, CRL Sign
            X509v3 Subject Key Identifier: 
                DF:55:7A:2C:90:B2:98:F3:0D:CE:26:2B:F6:BA:AF:64:27:54:A0:A6
            X509v3 Authority Key Identifier: 
                85:FC:C9:21:1E:50:3A:43:A7:CD:36:37:44:92:E0:88:8A:1B:29:39
    Signature Algorithm: sha256WithRSAEncryption
    Signature Value:
        90:b0:54:2d:a7:7d:eb:03:3c:79:7d:19:e1:c1:74:f9:3e:f7:
        2b:b6:63:04:fa:69:26:f8:35:14:69:04:74:f0:0d:a2:6a:08:
        77:42:03:e3:39:1d:db:1b:35:7c:e1:8f:36:33:68:19:78:35:
        5f:73:bb:06:59:69:21:ac:aa:4f:57:7a:c4:a5:75:14:0b:e3:
        d6:c4:c5:91:4f:71:e6:49:bc:02:fa:91:0f:ea:62:d1:cf:47:
        f0:46:e7:ed:1c:c4:9d:d4:37:61:ae:df:cc:77:4a:19:56:fb:
        c0:ff:41:e4:7d:47:95:5d:ba:cc:d8:96:ad:e7:d0:05:fc:41:
        ba:77:c7:14:f4:02:ab:27:df:17:1a:34:38:16:4f:f6:4c:bb:
        6f:37:b4:03:77:d6:b3:38:f3:e8:5b:2f:e0:90:42:e2:82:91:
        b9:cf:40:cb:9d:42:d2:e3:b5:7c:5c:70:b9:dc:a5:0a:4a:f9:
        31:a1:0e:30:2a:52:d9:87:cc:a6:1c:cc:d1:5d:15:37:d6:f6:
        f8:b0:af:47:6e:47:de:8e:81:ba:3e:68:57:f5:9c:ae:58:96:
        dd:ef:3c:a7:e4:e3:8e:d7:b5:0f:5a:77:61:bb:6f:c0:d5:ba:
        9a:e7:7e:88:d7:a4:43:5d:c6:45:a9:56:86:ca:af:be:62:96:
        6a:a0:ae:c7
-----BEGIN CERTIFICATE-----
MIIDKDCCAhCgAwIBAgIUJ1YSnbqrms+8U7dZT8S2ZE9AK48wDQYJKoZIhvcNAQEL
BQAwFzEVMBMGA1UEAwwMVGVzdCBSb290IENBMCAXDTI0MDgyMDA2MDgxMVoYDzk5
OTkxMjMxMjI1OTU5WjAeMRwwGgYDVQQDDBNUZXN0IENvZGVTaWduaW5nIENBMIIB
IjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAi+NFalFkdSKmWnDRCuF6FJXC
5gaKeCc3Cgn3ytbQsgwpMWyjXzXU0i+6tc2zvvEZ7KN8Wm6pKp7FiJlNR1XOgkxm
W7bJHQEz3IoY8bmlqHcCDS/3bTmZxdY6Rn5tNespJZ4Fi7czrar1N2tXPc+3q8FJ
b/z/nT2Ol1zf+UyKOehx9z9IM0cklSoLshwi/kyDfy0T4ZKEJhfVF3Uh12AJClaa
pStUDervpQ8bYltQzmyO9i+tUm8+SYjz3KPUOB65GkHZ6Xarm5RxNhgBk7CPGjXZ
w+ZTy5A9KVvCcCvWzAt4anMgMqRX3X7XwL5gl9He64Rwk2h1XfrcUWdI4aYtXQID
AQABo2MwYTAPBgNVHRMBAf8EBTADAQH/MA4GA1UdDwEB/wQEAwIBhjAdBgNVHQ4E
FgQU31V6LJCymPMNziYr9rqvZCdUoKYwHwYDVR0jBBgwFoAUhfzJIR5QOkOnzTY3
RJLgiIobKTkwDQYJKoZIhvcNAQELBQADggEBAJCwVC2nfesDPHl9GeHBdPk+9yu2
YwT6aSb4NRRpBHTwDaJqCHdCA+M5HdsbNXzhjzYzaBl4NV9zuwZZaSGsqk9XesSl
dRQL49bExZFPceZJvAL6kQ/qYtHPR/BG5+0cxJ3UN2Gu38x3ShlW+8D/QeR9R5Vd
uszYlq3n0AX8Qbp3xxT0Aqsn3xcaNDgWT/ZMu283tAN31rM48+hbL+CQQuKCkbnP
QMudQtLjtXxccLncpQpK+TGhDjAqUtmHzKYczNFdFTfW9viwr0duR96Ogbo+aFf1
nK5Ylt3vPKfk447XtQ9ad2G7b8DVuprnfojXpENdxkWpVobKr75ilmqgrsc=
-----END CERTIFICATE-----
";

    public static readonly string TestCodeCaKeyPem = @"-----BEGIN PRIVATE KEY-----
MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQCL40VqUWR1IqZa
cNEK4XoUlcLmBop4JzcKCffK1tCyDCkxbKNfNdTSL7q1zbO+8Rnso3xabqkqnsWI
mU1HVc6CTGZbtskdATPcihjxuaWodwINL/dtOZnF1jpGfm016yklngWLtzOtqvU3
a1c9z7erwUlv/P+dPY6XXN/5TIo56HH3P0gzRySVKguyHCL+TIN/LRPhkoQmF9UX
dSHXYAkKVpqlK1QN6u+lDxtiW1DObI72L61Sbz5JiPPco9Q4HrkaQdnpdqublHE2
GAGTsI8aNdnD5lPLkD0pW8JwK9bMC3hqcyAypFfdftfAvmCX0d7rhHCTaHVd+txR
Z0jhpi1dAgMBAAECggEADzTBUvFODj2Z/7LLxncEIkC1UvPbEXyRyxGSUgZP4UrZ
H3pDuBHN9JsjnKejplnnC4Yp+lqVIQDpUBP6BosZS6iqZ8XSzshWrt6EH6Kik1F8
c5qHNuRQBuVmqEsN7BKIAoLO74UWpQY0abXNYfa2bIEyTm4eCyWp3mJGJn9fdzPY
JgYJCoQXbP6GezyOje43o3pHyHf2N3mQIDjl1gsrfD4is3dlIyU7jhBstarb2OqK
kfd9MqynPwnPlecfKkpX9sEIgqUEzocgcUqyxyS9wb8yOLgES/e/13f3FimGeeGO
luCKT+sX9EwxbkKj4tRybWyx3LnqW76iyYmkPqNUAQKBgQDDUnKoHYw8T/d2AUg4
9AKtwFPoz6BiJfNGk9JtrcRqncHVOBoN85bKGUArh98WPYmZGnDo5tnc7fv+bocR
+QJoeOmGz98keIQchGaNCCfq0OKoR185k/ztepuoEj4S87+ZBFdlMoQY91rn4jWw
0DMPwyeER7KskHPgKgHJRMtdwQKBgQC3WEBehRZmSnIi7F7Yju6l0cDCVDgJlciT
6YKTbY99YprYoStpqLB5XxsptI215wteoE6JaMkzhAwS6DGQB9xFF3aJTSOxjnCH
kBbKAPx2obDE0owYx76VbKMemkcBGWi/VFDfEAfQh2koRU4biBNWNJzE3C0e/KNa
um0yRVUunQKBgQC0iWCxQ557oO56hHTdL63KLBti9YapMarLcZbvCc6jPW4MRu7O
NnkKFIzbr5rkU7z7ZxU2MSruqophgogWdLNlHV283ibC7yItubOQaBl9UdYu9Mlv
zvnaB5oYn8QAgIx4QysEvsyaxwefjddStx57U0cTXbIpDtwMNsev4YaYAQKBgQCm
8JNDvRilMRn11JWTx6likLdb+kU/7QNCMrdzN6oUd+kYYL/fp/pvuPpJoh36SBKh
KYP5N9EjuBMqUAN46r9UpcWHxRZAALtTpA4sBnPaLOWAgVmQ4qcU9WMdZUpLpPAs
bkBoqvcCl5lXUquJBADfWG56DSSEd3LiFKeCw4CJPQKBgGmKU4vkuFE8ltlbxV/X
IvnAYrHBqh98YXf8EnXVEhN0/FY3+RTNOD0dLQyIiGI7IQH6/KYOws0x5pnsVSMg
lblaLD1aiaG5NR/pzeKajaNcWcfPvN+xbrEOhD/4NuGaEhZv4jUyhBMdbCQ64LC2
KLOhkmd5gEOS+Vl8Qe+zSyLD
-----END PRIVATE KEY-----
";

    public static readonly byte[] TestCodeCaCrl0Der = Convert.FromHexString("308201783062020101300d06092a864886f70d01010b0500301e311c301a06035504030c135465737420436f64655369676e696e67204341170d3234303832303036303831325a180f39393939313233313232353935395aa00e300c300a0603551d140403020100300d06092a864886f70d01010b0500038201010018c48143ec13c6e7bb992d7be91bcea87da0962ecbc324b3255254a3b3a9cbb6019d8f451ec465bd7d6fa1a13be4d50bc26318d143fc5fe674d39f4ab119df0c872deabe5c0853489ae947438a34ec1440494332f9fae6d19c955109cd6861599182d6e97b8f0027d633eff262b501bc34af1aa6fab76c6d8f8c27ac905e2e8f9fbfc4ba3cef69752bff657458032412b4d878aab386debcdb93fc88875b31f041695e85776afe78a3ebea8253172a94ed69823335976589c11d199a8ed67b04b1fa94b406b9256e2137a21a431f1bc01371ed65ea2cc4086aea95d62164acf6fbaecf22dce369e6d60a7b4d39406dc06309d0c9d67cb68435bfdbb5a1e712a6");
    public static readonly byte[] TestCodeCaCrl1Der = Convert.FromHexString("308201a230818b020101300d06092a864886f70d01010b0500301e311c301a06035504030c135465737420436f64655369676e696e67204341170d3234303832303036303831325a180f39393939313233313232353935395a302730250214298b6fc5d1272b9fb5d715fe9033e799ce51a5d5170d3234303832303036303831325aa00e300c300a0603551d140403020101300d06092a864886f70d01010b0500038201010033854c47e4a6aede7daf8461f2929294a37670fbab3b4c4ca59f116d39733da07d88b6b6c9b7e7b6b97dfad49426b9924b772fde39753b76dc6dfae43aae19a614b90d5d681c75ef85a476b308aa8a277642828598d242a48c0f984823735b650ad08daf6942160c4be4d166ff98e26191d0a4ab0c8ae157fa0f40ddb38db0418d3fdc9e69070b469320194937c44c215dbe58256f436321cd07a3f79af8a0bed983fdd13dc9245bcf86bf296ad1fc751dbf8d2385f06568b1146e42e97ad2b05ac04a6ece90b4b47150cf83142cb4430c07a5793704cf72463b888860793a7ccfadcfc94cced1f5b4fa1d6c609e5697763f7c595ee58f18fd36bf44a369cf45");

    public static readonly string TestCodePem = @"Certificate:
    Data:
        Version: 3 (0x2)
        Serial Number:
            29:8b:6f:c5:d1:27:2b:9f:b5:d7:15:fe:90:33:e7:99:ce:51:a5:d5
        Signature Algorithm: sha256WithRSAEncryption
        Issuer: CN=Test CodeSigning CA
        Validity
            Not Before: Aug 20 06:08:11 2024 GMT
            Not After : Dec 31 22:59:59 9999 GMT
        Subject: CN=Test CodeSigning
        Subject Public Key Info:
            Public Key Algorithm: rsaEncryption
                Public-Key: (2048 bit)
                Modulus:
                    00:dc:8e:c7:1d:6a:de:69:9d:d3:10:c7:35:b7:ae:
                    53:90:09:7a:e8:0d:72:e0:0d:f6:0e:d3:49:ed:1c:
                    0e:da:4e:6b:36:68:34:ef:68:fd:be:5e:46:2a:f1:
                    61:a1:65:70:9b:5e:f0:0f:72:56:94:56:85:29:d2:
                    1a:a7:14:ae:6d:6d:1e:f8:e7:94:7c:d6:90:dd:a4:
                    0c:54:e0:d5:b7:b4:2e:11:01:c3:15:ab:24:1c:d6:
                    a2:33:75:c4:55:0b:4a:98:f6:d9:12:c2:eb:90:97:
                    aa:8c:85:5a:82:2f:ae:8f:9d:42:7f:57:7a:0b:8b:
                    69:28:a4:fb:4f:ba:3e:80:c0:6f:41:6b:a6:b1:61:
                    3a:3c:38:89:42:49:e1:fb:5c:18:b5:16:93:08:94:
                    58:d8:57:33:a0:40:da:59:57:d2:ad:ff:88:be:2b:
                    aa:49:a4:4b:ed:b9:b5:7d:82:28:f1:45:b1:33:89:
                    59:5d:f7:e6:8a:04:30:20:b3:44:08:e8:c0:b9:84:
                    15:12:10:ce:95:a6:2f:67:70:75:5f:6a:78:a1:53:
                    35:c7:c1:37:7d:70:ac:64:7c:29:c8:68:f2:e8:12:
                    18:7a:70:f1:92:c6:33:78:25:fd:63:2b:fb:40:66:
                    74:5e:7b:6e:7f:fb:01:fe:57:76:80:76:e3:e3:09:
                    5b:b1
                Exponent: 65537 (0x10001)
        X509v3 extensions:
            X509v3 Basic Constraints: 
                CA:FALSE
            X509v3 Extended Key Usage: critical
                Code Signing, E-mail Protection
            X509v3 Key Usage: critical
                Digital Signature, Non Repudiation
            X509v3 Subject Key Identifier: 
                31:F9:A8:23:DD:15:FB:A4:B4:EF:A6:0B:AD:A0:7B:89:4A:7B:0D:FB
            X509v3 Authority Key Identifier: 
                DF:55:7A:2C:90:B2:98:F3:0D:CE:26:2B:F6:BA:AF:64:27:54:A0:A6
    Signature Algorithm: sha256WithRSAEncryption
    Signature Value:
        8b:a0:83:cb:c2:51:06:5b:88:2b:18:71:59:96:a0:0a:bb:6e:
        4c:8d:9b:80:78:8e:27:73:56:5b:e6:22:1a:e6:75:9b:6a:24:
        45:9d:07:dd:5f:8a:d5:5c:69:21:97:43:74:4f:1e:a2:5e:c5:
        f3:9c:cf:96:27:29:c5:a0:57:20:a1:3e:fd:2a:d3:b8:e0:5b:
        8e:0d:86:5e:8b:0a:18:d0:bc:af:2c:31:57:7c:d6:10:8e:ec:
        a5:a8:37:d3:1a:09:eb:cb:f2:e9:c6:59:01:a3:70:6c:fe:26:
        60:1d:bb:42:1e:76:9d:46:ae:c2:e4:fb:e0:89:a9:52:42:9b:
        ad:4a:f5:fc:f9:0d:9a:21:63:07:41:bc:02:f1:ba:ca:50:73:
        85:ac:95:53:fc:a3:17:01:c0:3c:d1:8f:b7:88:f1:d2:94:59:
        33:18:0c:17:5e:1b:62:3a:02:e2:71:8e:4f:92:9b:de:84:8e:
        8f:03:61:e8:4b:54:2e:1c:14:21:6a:98:e7:52:e3:2b:a7:8d:
        92:e8:48:f4:24:5d:a9:9a:1d:86:29:1b:fe:90:f7:4b:26:bf:
        2f:d8:91:0b:35:b7:96:81:44:99:9e:9c:00:53:69:07:b5:89:
        c6:ed:22:a5:48:7c:d3:97:51:53:62:0e:3f:0b:21:75:bb:77:
        e4:07:b4:7e
-----BEGIN CERTIFICATE-----
MIIDSDCCAjCgAwIBAgIUKYtvxdEnK5+11xX+kDPnmc5RpdUwDQYJKoZIhvcNAQEL
BQAwHjEcMBoGA1UEAwwTVGVzdCBDb2RlU2lnbmluZyBDQTAgFw0yNDA4MjAwNjA4
MTFaGA85OTk5MTIzMTIyNTk1OVowGzEZMBcGA1UEAwwQVGVzdCBDb2RlU2lnbmlu
ZzCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBANyOxx1q3mmd0xDHNbeu
U5AJeugNcuAN9g7TSe0cDtpOazZoNO9o/b5eRirxYaFlcJte8A9yVpRWhSnSGqcU
rm1tHvjnlHzWkN2kDFTg1be0LhEBwxWrJBzWojN1xFULSpj22RLC65CXqoyFWoIv
ro+dQn9XeguLaSik+0+6PoDAb0FrprFhOjw4iUJJ4ftcGLUWkwiUWNhXM6BA2llX
0q3/iL4rqkmkS+25tX2CKPFFsTOJWV335ooEMCCzRAjowLmEFRIQzpWmL2dwdV9q
eKFTNcfBN31wrGR8Kcho8ugSGHpw8ZLGM3gl/WMr+0BmdF57bn/7Af5XdoB24+MJ
W7ECAwEAAaN/MH0wCQYDVR0TBAIwADAgBgNVHSUBAf8EFjAUBggrBgEFBQcDAwYI
KwYBBQUHAwQwDgYDVR0PAQH/BAQDAgbAMB0GA1UdDgQWBBQx+agj3RX7pLTvpgut
oHuJSnsN+zAfBgNVHSMEGDAWgBTfVXoskLKY8w3OJiv2uq9kJ1SgpjANBgkqhkiG
9w0BAQsFAAOCAQEAi6CDy8JRBluIKxhxWZagCrtuTI2bgHiOJ3NWW+YiGuZ1m2ok
RZ0H3V+K1VxpIZdDdE8eol7F85zPlicpxaBXIKE+/SrTuOBbjg2GXosKGNC8rywx
V3zWEI7spag30xoJ68vy6cZZAaNwbP4mYB27Qh52nUauwuT74ImpUkKbrUr1/PkN
miFjB0G8AvG6ylBzhayVU/yjFwHAPNGPt4jx0pRZMxgMF14bYjoC4nGOT5Kb3oSO
jwNh6EtULhwUIWqY51LjK6eNkuhI9CRdqZodhikb/pD3Sya/L9iRCzW3loFEmZ6c
AFNpB7WJxu0ipUh805dRU2IOPwshdbt35Ae0fg==
-----END CERTIFICATE-----
";

    public static readonly string TestCodeKeyPem = @"-----BEGIN PRIVATE KEY-----
MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDcjscdat5pndMQ
xzW3rlOQCXroDXLgDfYO00ntHA7aTms2aDTvaP2+XkYq8WGhZXCbXvAPclaUVoUp
0hqnFK5tbR7455R81pDdpAxU4NW3tC4RAcMVqyQc1qIzdcRVC0qY9tkSwuuQl6qM
hVqCL66PnUJ/V3oLi2kopPtPuj6AwG9Ba6axYTo8OIlCSeH7XBi1FpMIlFjYVzOg
QNpZV9Kt/4i+K6pJpEvtubV9gijxRbEziVld9+aKBDAgs0QI6MC5hBUSEM6Vpi9n
cHVfanihUzXHwTd9cKxkfCnIaPLoEhh6cPGSxjN4Jf1jK/tAZnRee25/+wH+V3aA
duPjCVuxAgMBAAECggEAHejw4LDm84y3HE3fn4axB1X5yHFWdEMAbNdDbbfB3eIH
kQvFrffffUr8mgM8+a2vfSp9RKL9UcV/7oFzIthfpTBIpHmHPfy9DYmbMIGDChQR
sTW7dntMfOqweWkNlHjhB2hmXLB5UJT4yHCnKwN7WTd61pkO0HT58EybVh8MLdpq
RdlsW7D6HNBM9w2a6tmVfyuAPC6ickuAnoKbgdXmqzJReTyjhxVMWXjvVuDqwqW3
yMA3cwqgIgNcNvffgJmSlMHqFe52clH/vbcYvNQrgbKDo8Dal4JMS5NP3C50ET/F
EtpE3PmUu+H29+r16N6WLttXS55qXn49b9/j0triLwKBgQDejEerZbTN27MZrbEP
qUtxygeJwPXCPPtTOIiFC76EUsTSGO6UwIFYioLrbSdzqesm38eJqDlH98pN6rPu
I8r23EcA7ZHNZNZjHKz01D1/EFvgr/Q/8NJvY+2wuJDXbTMlJLpUDWRWvONMZTmB
VMVvvW9NB3W0XdfApneDXy3WvwKBgQD9temh01VEPnZgopgLcPGEzm1YTLUNV09K
nxNl7MohLHcysMHWIW5R1GhS9m/yi0Z639y1gTtaL3D3Nt+YBDkauSzsgxMFJKS0
aC2BCRUzlRWnYTWIIy6y0PQ6Bf+XVV9D6XEW5WsHy5ZB7NqPIBxi4QkwNJf4CXlG
SdYFvrpZjwKBgQCFmlGsY/KXYz2yPMP/UvMn7NF5sY3YfiOjYl0TAmntpoLiXnVc
d/Uu4niWPYFhvrCdNdrkmUlKG0goXq6GWJaQfM1jkTvmwpKyDZUHpiMFFR0TVo6b
lEfA94zKewL5UWVS9ymlLCUArYUHZ/2N1wZwfplC9SeEkJHl5gn5pDowoQKBgBI0
3ro1LzZBgT84C88uYLaWFbSzdH6rmE4TklHLJ0owJbGmi5JxPbhzlLT3aFswcXNZ
4z3HM1oqc2HaNztyeiKXzUh/s3q4AXXM0A8ldrcH6NDlIfFODQjaA9yyF4BlqPt3
ohzxg7fAfVznwVtqA395BK7H52vuShyoKcqpwDezAoGBAJsOnNc0/+ShabraHo6T
t1uvZoS4hqrVSzg+LKAbQHTxfEJbfkxYuFVw/fNOIsXBl2f/lkpjUk4nL94kfWeF
EpMygFmGza9aR3yBRqxW/1ObgLEp1h1ZUcRkmjKceKu71xX4J2eBnKt3a4w2Fdq2
JLwlKQQYWrmfEM9nYePunYNe
-----END PRIVATE KEY-----
";
}