import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CertificateCredentials, KeyCredentials, KeyCredentialsKind, TokenCredentials, UsernamePasswordCredentials } from '../services/models';

@Component({
  selector: 'app-credentials-value',
  templateUrl: './credentials-value.component.html',
  styleUrl: './credentials-value.component.css',
})
export class CredentialsValueComponent {
  public KeyCredentialsKind = KeyCredentialsKind;

  readonly optionsTokenType: string[] = [
    "http://opcfoundation.org/UA/UserToken#JWT",
    "http://docs.oasis-open.org/wss/oasis-wss-kerberos-token-profile-1.1"
  ];

  @Input() keyPassword?: string;
  @Output() keyPasswordChange = new EventEmitter<string | undefined>();

  private _credentials: KeyCredentials = {};
  @Input() set credentials(value: KeyCredentials | undefined) {
    this._credentials = value ?? {}
    this._credentials.description ??= { kind: KeyCredentialsKind.None };
    this._credentials
  }
  get credentials(): KeyCredentials {
    return this._credentials;
  }
  @Output() credentialsChange = new EventEmitter<KeyCredentials | undefined>();

  get kind() {
    return this.credentials.description?.kind ?? KeyCredentialsKind.None;
  }

  set kind(value: KeyCredentialsKind) {
    if (this.kind != value) {
      this._credentials.description ??= { kind: KeyCredentialsKind.None };
      this._credentials.description.kind = value;
      this._credentials.value = { kind: value };
      this.credentialsChange.emit(this.credentials);
    }
  }

  get label() {
    return this.credentials.description?.label;
  }

  set label(value) {
    this._credentials.description!.label = value;
    this.credentialsChange.emit(this.credentials);
  }

  get details() {
    return this.credentials.description?.details;
  }

  set details(value) {
    this._credentials.description!.details = value;
    this.credentialsChange.emit(this.credentials);
  }

  get username() {
    if (this.kind == KeyCredentialsKind.UsernamePassword) {
      if (this.credentials.value) {
        return (this.credentials.value as UsernamePasswordCredentials).username;
      }
    }
    return undefined;
  }

  set username(value) {
    if (typeof this.credentials.value === 'string' || this.credentials.value instanceof String) {
      this._credentials.value = {};
    }
    this._credentials.value ??= {} as UsernamePasswordCredentials;
    (this._credentials.value as UsernamePasswordCredentials).username = value;
    this.credentialsChange.emit(this.credentials);
  }

  get password() {
    if (this.kind == KeyCredentialsKind.UsernamePassword) {
      if (this.credentials.value) {
        return (this.credentials.value as UsernamePasswordCredentials).password;
      }
    }
    return undefined;
  }

  set password(value) {
    if (typeof this.credentials.value === 'string' || this.credentials.value instanceof String) {
      this._credentials.value = {};
    }
    this._credentials.value ??= {} as UsernamePasswordCredentials;
    (this._credentials.value as UsernamePasswordCredentials).password = value;
    this.credentialsChange.emit(this.credentials);
  }

  get token() {
    if (this.kind == KeyCredentialsKind.Token) {
      if (this.credentials.value) {
        return (this.credentials.value as TokenCredentials).token;
      }
    }
    return undefined;
  }

  set token(value) {
    if (typeof this.credentials.value === 'string' || this.credentials.value instanceof String) {
      this._credentials.value = {};
    }
    this._credentials.value ??= {} as TokenCredentials;
    (this._credentials.value as TokenCredentials).token = value;
    this.credentialsChange.emit(this.credentials);
  }

  get tokenType() {
    if (this.kind == KeyCredentialsKind.Token) {
      if (this.credentials.value) {
        return (this.credentials.value as TokenCredentials).tokenType;
      }
    }
    return undefined;
  }

  set tokenType(value) {
    if (typeof this.credentials.value === 'string' || this.credentials.value instanceof String) {
      this._credentials.value = {};
    }
    this._credentials.value ??= {} as TokenCredentials;
    (this._credentials.value as TokenCredentials).tokenType = value;
    this.credentialsChange.emit(this.credentials);
  }

  get issuerEndpointUrl() {
    if (this.kind == KeyCredentialsKind.Token) {
      if (this.credentials.value) {
        return (this.credentials.value as TokenCredentials).issuerEndpointUrl;
      }
    }
    return undefined;
  }

  set issuerEndpointUrl(value) {
    if (typeof this.credentials.value === 'string' || this.credentials.value instanceof String) {
      this._credentials.value = {};
    }
    this._credentials.value ??= {} as TokenCredentials;
    (this._credentials.value as TokenCredentials).issuerEndpointUrl = value;
    this.credentialsChange.emit(this.credentials);
  }

  get certificate() {
    if (this.kind == KeyCredentialsKind.Certificate) {
      if (this.credentials.value) {
        return (this.credentials.value as CertificateCredentials).certificate;
      }
    }
    return undefined;
  }

  set certificate(value) {
    if (typeof this.credentials.value === 'string' || this.credentials.value instanceof String) {
      this._credentials.value = {};
    }
    this._credentials.value ??= {} as CertificateCredentials;
    (this._credentials.value as CertificateCredentials).certificate = value;
    this.credentialsChange.emit(this.credentials);
  }

  get keyPair() {
    if (this.kind == KeyCredentialsKind.Certificate) {
      if (this.credentials.value) {
        return (this.credentials.value as CertificateCredentials).keyPair;
      }
    }
    return undefined;
  }

  set keyPair(value) {
    if (typeof this.credentials.value === 'string' || this.credentials.value instanceof String) {
      this._credentials.value = {};
    }
    this._credentials.value ??= {} as CertificateCredentials;
    (this._credentials.value as CertificateCredentials).keyPair = value;
    this.credentialsChange.emit(this.credentials);
  }
}