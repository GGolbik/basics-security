import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ConfigExtensionDefault } from 'src/app/services/models';

@Component({
  selector: 'app-extensions',
  templateUrl: './extensions.component.html',
  styleUrls: ['./extensions.component.css']
})
export class ExtensionsComponent {

  _extensions: ConfigExtensionDefault[] = [];

  @Input() set config(list: ConfigExtensionDefault[] | undefined) {
    this._extensions = list ?? [];
  }

  @Output() configChange = new EventEmitter<ConfigExtensionDefault[] | undefined>();

  addEntry() {
    this._extensions.push({ critical: false, oid: "", value: "" } as ConfigExtensionDefault);
    this.fire();
  }

  deleteEntry(index: number) {
    this._extensions.splice(index, 1);
    this.fire();
  }

  setV(i:number, value:any) {
    this._extensions[i] = value.target.value;
    this.fire();
  }

  fire(){
    this.configChange.emit(this._extensions.length == 0 ? undefined : this._extensions);
  }
}
