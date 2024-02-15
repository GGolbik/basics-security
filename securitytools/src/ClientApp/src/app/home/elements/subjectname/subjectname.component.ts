import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ConfigSubjectName, Dictionary } from 'src/app/services/models';

@Component({
  selector: 'app-subjectname',
  templateUrl: './subjectname.component.html',
  styleUrls: ['./subjectname.component.css']
})
export class SubjectnameComponent {
  _config?: ConfigSubjectName;

  @Input() set config(config: ConfigSubjectName | undefined) {
    this._config = config;
    let dcs = this._config?.domainComponents ?? [];
    this._domainComponents = dcs.join('.');
  }

  @Output() configChange = new EventEmitter<ConfigSubjectName | undefined>();

  get config() : ConfigSubjectName {
    let config = this._config;
    config ??= {} as ConfigSubjectName;
    return config;
  }

  fire() {
    this.configChange.emit(this._config);
  }

  get commonName() {
    return this.config.commonName;
  }

  set commonName(v) {
    let config = this.config;
    if(v == null || v?.trim() == "") {
      delete config.commonName;
    } else {
      config.commonName = v;
    }
  
    this._config = config;
    this.fire();
  }

  get organizationalUnitName() {
    return this.config.organizationalUnitName;
  }

  set organizationalUnitName(v) {
    let config = this.config;
    if(v == null || v?.trim() == "") {
      delete config.organizationalUnitName;
    } else {
      config.organizationalUnitName = v;
    }
  
    this._config = config;
    this.fire();
  }

  get organizationName() {
    return this.config.organizationName;
  }

  set organizationName(v) {
    let config = this.config;
    if(v == null || v?.trim() == "") {
      delete config.organizationName;
    } else {
      config.organizationName = v;
    }
  
    this._config = config;
    this.fire();
  }

  get stateOrProvinceName() {
    return this.config.stateOrProvinceName;
  }

  set stateOrProvinceName(v) {
    let config = this.config;
    if(v == null || v?.trim() == "") {
      delete config.stateOrProvinceName;
    } else {
      config.stateOrProvinceName = v;
    }
  
    this._config = config;
    this.fire();
  }
  
  get countryName() {
    return this.config.countryName;
  }

  set countryName(v) {
    let config = this.config;
    if(v == null || v?.trim() == "") {
      delete config.countryName;
    } else {
      config.countryName = v;
    }
  
    this._config = config;
    this.fire();
  }

  get localityName() {
    return this.config.localityName;
  }

  set localityName(v) {
    let config = this.config;
    if(v == null || v?.trim() == "") {
      delete config.localityName;
    } else {
      config.localityName = v;
    }
  
    this._config = config;
    this.fire();
  }

  get title() {
    return this.config.title;
  }

  set title(v) {
    let config = this.config;
    if(v == null || v?.trim() == "") {
      delete config.title;
    } else {
      config.title = v;
    }
  
    this._config = config;
    this.fire();
  }

  get surname() {
    return this.config.surname;
  }

  set surname(v) {
    let config = this.config;
    if(v == null || v?.trim() == "") {
      delete config.surname;
    } else {
      config.surname = v;
    }
  
    this._config = config;
    this.fire();
  }

  get givenName() {
    return this.config.givenName;
  }

  set givenName(v) {
    let config = this.config;
    if(v == null || v?.trim() == "") {
      delete config.givenName;
    } else {
      config.givenName = v;
    }
  
    this._config = config;
    this.fire();
  }

  get initials() {
    return this.config.initials;
  }

  set initials(v) {
    let config = this.config;
    if(v == null || v?.trim() == "") {
      delete config.initials;
    } else {
      config.initials = v;
    }
  
    this._config = config;
    this.fire();
  }

  get pseudonym() {
    return this.config.pseudonym;
  }

  set pseudonym(v) {
    let config = this.config;
    if(v == null || v?.trim() == "") {
      delete config.pseudonym;
    } else {
      config.givenName = v;
    }
  
    this._config = config;
    this.fire();
  }

  get generationQualifier() {
    return this.config.generationQualifier;
  }

  set generationQualifier(v) {
    let config = this.config;
    if(v == null || v?.trim() == "") {
      delete config.generationQualifier;
    } else {
      config.generationQualifier = v;
    }
  
    this._config = config;
    this.fire();
  }

  get emailAddress() {
    return this.config.emailAddress;
  }

  set emailAddress(v) {
    let config = this.config;
    if(v == null || v?.trim() == "") {
      delete config.emailAddress;
    } else {
      config.emailAddress = v;
    }
  
    this._config = config;
    this.fire();
  }

  get oids() {
    return this.config.oids ?? [];
  }

  set oids(v) {
    let config = this.config;
    if(v == null || v?.length == 0) {
      delete config.oids;
    } else {
      config.oids = v;
    }
  
    this._config = config;
    this.fire();
  }


  _domainComponents: string = "";

  get domainComponents(): string {
    return this._domainComponents;
  }

  set domainComponents(dc: string) {
    this._domainComponents = dc;
    this.config.domainComponents = [];
    let dcs = dc.split(".");
    for (let i = 0; i < dcs.length; i++) {
      if (dcs[i].trim() == "") {
        continue;
      }
      this.config.domainComponents.push(dcs[i].trim());
    }
    if (this.config.domainComponents.length == 0) {
      this.config.domainComponents = undefined;
    }
  }

  addEntry() {
    let config = this.config;
    let oids = config.oids ?? [];
    oids.push({"": ""});
    config.oids = oids;

    this._config = config;
    this.fire();
  }

  deleteEntry(index: number) {
    let config = this.config;
  
    config.oids?.splice(index, 1);
    this.oids = config.oids ?? [];
  }

  getKey(entry: Dictionary<string, string>) {
    return Object.keys(entry)[0];
  }

  getValue(entry: Dictionary<string, string>) {
    return entry[this.getKey(entry)];
  }

  setK(i: number, value:any) {
    let entry = {} as Dictionary<string, string>;
    entry[value.target.value as string] = this.config.oids![i][this.getKey(this.config.oids![i])];
    this.config.oids![i] = entry;
    this.fire();
  }

  setV(i:number, value:any) {
    this.config.oids![i][this.getKey(this.config.oids![i])] = value.target.value;
    this.fire();
  }
}