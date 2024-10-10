import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ConfigEdiPartyName, ConfigOtherName, ConfigSubjectAlternativeName } from '../../../../services/models';

@Component({
  selector: 'app-subject-alternative-name',
  templateUrl: './subject-alternative-name.component.html',
  styleUrls: ['./subject-alternative-name.component.css']
})
export class SubjectAlternativeNameComponent {
  otherNames = [] as ConfigOtherName[];
  emailAddresses = [] as string[];
  dnsNames = [] as string[];
  x400Addresses = [] as string[];
  directoryNames = [] as string[];
  ediPartyNames = [] as ConfigEdiPartyName[];
  uris = [] as string[];
  iPAddresses = [] as string[];
  registeredIds = [] as string[];
  critical = false;

  @Input() set config(config: ConfigSubjectAlternativeName | undefined) {
    this.critical = config?.critical ?? false;
    this.otherNames = config?.otherNames ?? [];
    this.emailAddresses = config?.emailAddresses ?? [];
    this.dnsNames = config?.dnsNames ?? [];
    this.x400Addresses = config?.x400Addresses ?? [];
    this.directoryNames = config?.directoryNames ?? [];
    this.ediPartyNames = config?.ediPartyNames ?? [];
    this.uris = config?.uris ?? [];
    this.iPAddresses = config?.iPAddresses ?? [];
    this.registeredIds = config?.registeredIds ?? [];
  }

  @Output() configChange = new EventEmitter<ConfigSubjectAlternativeName | undefined>();

  fire() {
    let config = { critical: this.critical } as ConfigSubjectAlternativeName;
    if (this.otherNames.length > 0) {
      config.otherNames = this.otherNames;
    }
    if (this.emailAddresses.length > 0) {
      config.emailAddresses = this.emailAddresses;
    }
    if (this.dnsNames.length > 0) {
      config.dnsNames = this.dnsNames;
    }
    if (this.x400Addresses.length > 0) {
      config.x400Addresses = this.x400Addresses;
    }
    if (this.directoryNames.length > 0) {
      config.directoryNames = this.directoryNames;
    }
    if (this.ediPartyNames.length > 0) {
      config.ediPartyNames = this.ediPartyNames;
    }
    if (this.uris.length > 0) {
      config.uris = this.uris;
    }
    if (this.iPAddresses.length > 0) {
      config.iPAddresses = this.iPAddresses;
    }
    if (this.registeredIds.length > 0) {
      config.registeredIds = this.registeredIds;
    }

    this.configChange.emit(Object.keys(config).length > 0 ? config : undefined);
  }
}
