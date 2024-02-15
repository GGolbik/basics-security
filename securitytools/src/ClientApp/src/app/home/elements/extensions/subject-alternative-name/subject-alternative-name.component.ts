import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ConfigSubjectAlternativeName } from 'src/app/services/models';

@Component({
  selector: 'app-subject-alternative-name',
  templateUrl: './subject-alternative-name.component.html',
  styleUrls: ['./subject-alternative-name.component.css']
})
export class SubjectAlternativeNameComponent {
  emailAddresses = [] as string[];
  dnsNames = [] as string[];
  iPAddresses = [] as string[];
  uris = [] as string[];
  userPrincipalNames = [] as string[];
  critical = false;

  @Input() set config(config: ConfigSubjectAlternativeName | undefined) {
    this.critical = config?.critical ?? false;
    this.emailAddresses = config?.emailAddresses ?? [];
    this.dnsNames = config?.dnsNames ?? [];
    this.iPAddresses = config?.iPAddresses ?? [];
    this.uris = config?.uris ?? [];
    this.userPrincipalNames = config?.userPrincipalNames ?? [];
  }

  @Output() configChange = new EventEmitter<ConfigSubjectAlternativeName | undefined>();

  fire() {
    let config = { critical: this.critical } as ConfigSubjectAlternativeName;
    if (this.emailAddresses.length > 0) {
      config.emailAddresses = this.emailAddresses;
    }
    if (this.dnsNames.length > 0) {
      config.dnsNames = this.dnsNames;
    }
    if (this.iPAddresses.length > 0) {
      config.iPAddresses = this.iPAddresses;
    }
    if (this.uris.length > 0) {
      config.uris = this.uris;
    }
    if (this.userPrincipalNames.length > 0) {
      config.userPrincipalNames = this.userPrincipalNames;
    }

    this.configChange.emit(Object.keys(config).length > 0 ? config : undefined);
  }
}
