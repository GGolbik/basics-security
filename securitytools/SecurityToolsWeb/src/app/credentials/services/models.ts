
export type Dictionary<K extends string | number | symbol, V> = Record<K, V>;

export enum KeyCredentialsKind {
    None = 0,
    Anonymous = 1,
    UsernamePassword = 2,
    Certificate = 3,
    Token = 4,
}

export interface KeyCredentialsDescription {
    label?: string;
    details?: string;
    kind?: KeyCredentialsKind;
    encrypted?: boolean;
}

export interface KeyCredentials {
    description?: KeyCredentialsDescription;
    value?: AnonymousCredentials | UsernamePasswordCredentials | CertificateCredentials | TokenCredentials | string;
}

export interface KeyCredentialsValue {
    kind?: KeyCredentialsKind;
}

export interface AnonymousCredentials extends KeyCredentialsValue {

}

export interface UsernamePasswordCredentials extends KeyCredentialsValue {
    username?: string;
    password?: string;
}

export interface CertificateCredentials extends KeyCredentialsValue {
    certificate?: string;
    keyPair?: string;
}

export interface TokenCredentials extends KeyCredentialsValue {
    token?: string;
    tokenType?: string;
    issuerEndpointUrl?: string;
}