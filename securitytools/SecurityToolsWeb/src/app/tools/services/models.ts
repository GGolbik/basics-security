// ##############################################
// Helper

import { ActivatedRoute, Router } from "@angular/router";

export type Base64 = string;
export type DateTime = string;
export type Dictionary<K extends string | number | symbol, V> = Record<K, V>;
export type EccurveNameOrOid = string;
export type HashAlgorithmNameOrOid = string;

// ng add @angular/material-moment-adapter
// ng add moment 
export const MY_FORMATS = {
    parse: {
      dateInput: 'YYYY-MM-DD',
    },
    display: {
      dateInput: 'YYYY-MM-DD',
      monthYearLabel: 'MMM YYYY',
      dateA11yLabel: 'LL',
      monthYearA11yLabel: 'MMMM YYYY',
    },
  };

export const EccurvesNames = {
    'brainpoolP160r1': '1.3.36.3.3.2.8.1.1.1',
    'brainpoolP160t1': '1.3.36.3.3.2.8.1.1.2',
    'brainpoolP192r1': '1.3.36.3.3.2.8.1.1.3',
    'brainpoolP192t1': '1.3.36.3.3.2.8.1.1.4',
    'brainpoolP224r1': '1.3.36.3.3.2.8.1.1.5',
    'brainpoolP224t1': '1.3.36.3.3.2.8.1.1.6',
    'brainpoolP256r1': '1.3.36.3.3.2.8.1.1.7',
    'brainpoolP256t1': '1.3.36.3.3.2.8.1.1.8',
    'brainpoolP320r1': '1.3.36.3.3.2.8.1.1.9',
    'brainpoolP320t1': '1.3.36.3.3.2.8.1.1.10',
    'brainpoolP384r1': '1.3.36.3.3.2.8.1.1.11',
    'brainpoolP384t1': '1.3.36.3.3.2.8.1.1.12',
    'brainpoolP512r1': '1.3.36.3.3.2.8.1.1.13',
    'brainpoolP512t1': '1.3.36.3.3.2.8.1.1.14',
    'nistP256': '1.2.840.10045.3.1.7',
    'nistP384': '1.3.132.0.34',
    'nistP521': '1.3.132.0.35',
}

export const HashAlgorithmNames = {
    'SHA1': '1.3.14.3.2.26',
    'SHA256': '2.16.840.1.101.3.4.2.1',
    'SHA384': '2.16.840.1.101.3.4.2.2',
    'SHA512': '2.16.840.1.101.3.4.2.3',
} as Dictionary<HashAlgorithmNameOrOid, HashAlgorithmNameOrOid>;

export const TextEncodingNames = {
    'ISO-8859-1': '28591',
    'US-ASCII': '20127',
    'UTF-8': '65001',
} as Dictionary<EccurveNameOrOid, EccurveNameOrOid>;

export interface ExecuteProgress {
    loaded: number;
    total: number;
    progress: number;
}

export class FragmentParams {
    readonly tabIndexkey = "tabindex";
    tabIndex = 0;

    readonly stepperIndexkey = "stepperindex";
    stepperIndex = 0;

    static parse(route: ActivatedRoute): FragmentParams {
        let result = new FragmentParams();

        const paramsString = route.snapshot.fragment ?? "";
        const searchParams = new URLSearchParams(paramsString);

        if (searchParams.has(result.tabIndexkey)) {
            result.tabIndex = FragmentParams.toNumber(searchParams.get(result.tabIndexkey));
        }

        if (searchParams.has(result.stepperIndexkey)) {
            //result.stepperIndex = FragmentParams.toNumber(searchParams.get(result.stepperIndexkey));
        }
        return result;
    }

    private static toNumber(value: string | null) {
        if (value == null) {
            return 0;
        }
        let index = Number(value);
        if (isNaN(index)) {
            return 0;
        } else {
            return index;
        }
    }

    updateUrl(router: Router, url = "/tools") {
        let params = new URLSearchParams();
        params.set(this.tabIndexkey, this.tabIndex + "");
        //params.set(this.stepperIndexkey, this.stepperIndex + "");
        router.navigate([url], {
            fragment: params.toString()
        });
    }
}

// ##############################################
// External

export enum CertificateRequestLoadOptions {
    Default = 0,
    SkipSignatureValidation = 1,
    UnsafeLoadCertificateExtensions = 2
}

export enum X509ContentType {
    None = 0,
    Cert = 1,
    SerializedCert = 2,
    Pkcs12 = 3,
    SerializedStore = 4,
    Pkcs7 = 5,
    Authenticode = 6
}

export enum X509FindType {
    FindByIndex = -1,
    FindByThumbprint = 0,
    FindBySubjectName = 1,
    FindBySubjectDistinguishedName = 2,
    FindByIssuerName = 3,
    FindByIssuerDistinguishedName = 4,
    FindBySerialNumber = 5,
    FindByTimeValid = 6,
    FindByTimeNotYetValid = 7,
    FindByTimeExpired = 8,
    FindByTemplateName = 9,
    FindByApplicationPolicy = 10,
    FindByCertificatePolicy = 11,
    FindByExtension = 12,
    FindByKeyUsage = 13,
    FindBySubjectKeyIdentifier = 14
}

export enum X509KeyUsageFlags {
    None = 0,
    EncipherOnly = 1,
    CrlSign = 2,
    KeyCertSign = 4,
    KeyAgreement = 8,
    DataEncipherment = 16,
    KeyEncipherment = 32,
    NonRepudiation = 64,
    DigitalSignature = 128,
    DecipherOnly = 32768
}

export enum X509RevocationReason {
    Unspecified = 0,
    KeyCompromise = 1,
    CACompromise = 2,
    AffiliationChanged = 3,
    Superseded = 4,
    CessationOfOperation = 5,
    CertificateHold = 6,
    RemoveFromCrl = 8,
    PrivilegeWithdrawn = 9,
    AACompromise = 10,
    WeakAlgorithmOrKey = 11
}

// ##############################################
// Web

export enum MediaType {
    Default = 0,
    Zip = 1,
    Tar = 2,
    TarGz = 3,
}

/**
 * A machine-readable format for specifying errors in HTTP API responses based on https://tools.ietf.org/html/rfc7807.
 */
export interface ProblemDetails {
    type: string;
    title: string;
    detail?: string;
    status?: number;
}

export interface ProgramInfo {
    title?: string;
    manufacturer?: string;
    description?: string;
    copyright?: string;
    major: number;
    minor: number;
    patch: number;
    build: number;
    releaseDate?: string;
    version: string;
}

// ##############################################
// Config

export interface ConfigAuthorityKeyIdentifierExtension extends ConfigExtension {
    includeKeyIdentifier?: boolean;
    includeIssuerAndSerial?: boolean;
}

export interface ConfigBasicConstraintsExtension extends ConfigExtension {
    certificateAuthority?: boolean;
    hasPathLengthConstraint?: boolean;
    pathLengthConstraint?: number;
}

export interface ConfigCert {
    schemaVersion?: string;
    keyPair?: ConfigKeyPair;
    issuer?: X509File;
    csrLoadOptions?: CertificateRequestLoadOptions;
    csr?: ConfigCsr;
    cert?: X509File;
    serialNumber?: Base64;
    validity?: ConfigValidity;
    extensions?: ConfigExtensions;
    replaceExtensions?: boolean;
    hashAlgorithm?: HashAlgorithmNameOrOid;
}

export interface ConfigCrl {
    schemaVersion?: string;
    crlNumber?: Base64;
    crl?: X509File;
    crlEntries?: CrlEntry[];
    issuer?: X509File;
    keyPair?: ConfigKeyPair;
    validity?: ConfigCrlValidity;
    hashAlgorithm?: HashAlgorithmNameOrOid;
}

export interface ConfigCsr {
    schemaVersion?: string;
    subjectName?: ConfigSubjectName;
    keyPair?: ConfigKeyPair;
    hashAlgorithm?: HashAlgorithmNameOrOid;
    extensions?: ConfigExtensions;
    csr?: X509File;
}

export interface ConfigExtendedKeyUsageExtension extends ConfigExtension {
    extendedKeyUsages: ExtendedKeyUsageFlags;
    oids?: string[];
}

export interface ConfigExtension {
    schemaVersion?: string;
    critical?: boolean;
}

export interface ConfigExtensionDefault extends ConfigExtension {
    oid?: string;
    value?: Base64;
}

export interface ConfigExtensions {
    schemaVersion?: string;
    keyUsage?: ConfigKeyUsageExtension;
    extendedKeyUsage?: ConfigExtendedKeyUsageExtension;
    subjectAlternativeName?: ConfigSubjectAlternativeName;
    basicConstraints?: ConfigBasicConstraintsExtension;
    subjectKeyIdentifier?: ConfigSubjectKeyIdentifierExtension;
    authorityKeyIdentifier?: ConfigAuthorityKeyIdentifierExtension;
    extensions?: ConfigExtensionDefault[];
}

export interface ConfigKeyPair {
    schemaVersion?: string;
    signatureAlgorithm?: SignatureAlgorithmName;
    privateKey?: X509File;
    keySize?: number;
    eccurve?: EccurveNameOrOid;
    publicKey?: X509File;
}

export interface ConfigKeyUsageExtension extends ConfigExtension {
    keyUsages: X509KeyUsageFlags;
}

export interface ConfigSubjectAlternativeName extends ConfigExtension {
    otherNames?: ConfigOtherName[];
    emailAddresses?: string[];
    dnsNames?: string[];
    x400Addresses?: string[];
    directoryNames?: string[];
    ediPartyNames?: ConfigEdiPartyName[];
    uris?: string[];
    iPAddresses?: string[];
    registeredIds?: string[];
}

export interface ConfigOtherName {
    typeId?: string;
    value?: string;
}

export interface ConfigEdiPartyName {
    nameAssigner?: string;
    partyName?: string;
}

export interface ConfigSubjectKeyIdentifierExtension extends ConfigExtension {

}

export interface ConfigSubjectName {
    schemaVersion?: string;
    countryName?: string;
    organizationName?: string;
    organizationalUnitName?: string;
    stateOrProvinceName?: string;
    commonName?: string;
    domainComponents?: string[];
    localityName?: string;
    title?: string;
    surname?: string;
    givenName?: string;
    initials?: string;
    pseudonym?: string;
    generationQualifier?: string;
    emailAddress?: string;
    oids?: Dictionary<string, string>[];
}

export interface ConfigTransform {
    schemaVersion?: string;
    mode?: TransformMode;
    input?: X509File[];
    output?: X509File[];
}

export interface ConfigValidity {
    schemaVersion?: string;
    notBefore?: DateTime;
    notAfter?: DateTime;
}

export interface ConfigCrlValidity {
    schemaVersion?: string;
    thisUpdate?: DateTime;
    nextUpdate?: DateTime;
}

export interface CrlEntry {
    schemaVersion?: string;
    cert?: X509File;
    serialNumber?: Base64;
    revocationTime?: DateTime;
    reason?: X509RevocationReason;
}

export enum ExtendedKeyUsageFlags {
    None = 0,
    ServerAuth = 1,
    ClientAuth = 2,
    CodeSigning = 4,
    EmailProtection = 8,
    TimeStamping = 16,
    OCSPSigning = 32
}


export enum SignatureAlgorithmName {
    Default = 0,
    Rsa = 1,
    //Dsa = 2,
    Ecdsa = 3,
    //Eddsa = 4,
    //Ecdh = 5
}

export enum TransformMode {
    None = 0,
    Store = 1,
    SinglePem = 2,
    Pem = 3,
    Der = 4,
    Config = 5,
}

export enum X509Encoding {
    Default = 0,
    Der = 1,
    Pem = 2
}

export interface X509File {
    schemaVersion?: string;
    data?: Base64;
    fileName?: string;
    fileFormat?: X509FileFormat;
    password?: string;
    alias?: string;
    aliasType?: X509FindType;
    hashAlgorithm?: HashAlgorithmNameOrOid;
}

export interface X509FileFormat {
    schemaVersion?: string;
    encoding?: X509Encoding;
    contentKind?: X509ContentType;
    codePage?: number;
}

// ##############################################
// Helper


export class ExtensionsDefaults {
    private constructor() {

    }
    public static readonly BasicConstraintsExtensionCA = { critical: true, certificateAuthority: true, hasPathLengthConstraint: false, pathLengthConstraint: 0 } as ConfigBasicConstraintsExtension;
    public static readonly BasicConstraintsExtensionEndEntity = { critical: false, certificateAuthority: false, hasPathLengthConstraint: false, pathLengthConstraint: 0 } as ConfigBasicConstraintsExtension;
    public static readonly AuthorityKeyIdentifierExtension = { critical: false, includeKeyIdentifier: true, includeIssuerAndSerial: false} as ConfigAuthorityKeyIdentifierExtension;
    public static readonly SubjectKeyIdentifierExtension = { critical: false } as ConfigSubjectKeyIdentifierExtension;
    public static readonly KeyUsageExtensionCa = { critical: true, keyUsages: X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign } as ConfigKeyUsageExtension;
    public static readonly KeyUsageExtensionServerAuth = { critical: true, keyUsages: X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.KeyAgreement } as ConfigKeyUsageExtension;
    public static readonly KeyUsageExtensionClientAuth = { critical: true, keyUsages: X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyAgreement } as ConfigKeyUsageExtension;
    public static readonly KeyUsageExtensionCodeSign = { critical: true, keyUsages: X509KeyUsageFlags.DigitalSignature } as ConfigKeyUsageExtension;
    public static readonly KeyUsageExtensionEmail = { critical: true, keyUsages: X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.KeyAgreement } as ConfigKeyUsageExtension;
    public static readonly KeyUsageExtensionTimestamping = { critical: true, keyUsages: X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation } as ConfigKeyUsageExtension;
}