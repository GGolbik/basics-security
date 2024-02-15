import { Component, EventEmitter, Input, Output } from '@angular/core';
import { base64ToHex, hexToBase64 } from 'src/main';

@Component({
  selector: 'app-byte-number',
  templateUrl: './byte-number.component.html',
  styleUrls: ['./byte-number.component.css']
})
export class ByteNumberComponent {

  israw = false;

  @Input() label?: string;

  @Input() inputName: string = "number";

  private _base64Value?: string;

  @Input() set config(config: string | undefined) {
    let updateNumeric = config != this._base64Value;
    this._base64Value = config;
    if (updateNumeric) {
      this.setNumericFromBase64(config);
    }
  }

  setNumericFromBase64(base64: string | undefined) {
    if (base64 == null || base64.length == 0) {
      this._numericValue = base64;
    } else {
      try {
        let hex = "0x" + base64ToHex(base64);
        if (this._numericValue == undefined || this._numericValue.trim().length == 0 || this._numericValue?.startsWith("0x")) {
          this._numericValue = hex;
        } else {
          let num = BigInt(hex);
          this._numericValue = num.toString(10);
        }
      } catch (err) {
        this._numericValue = base64;
      }
    }
  }

  @Output() configChange = new EventEmitter<string | undefined>();

  fire() {
    this.configChange.emit(this._base64Value)
  }

  get raw() {
    return this._base64Value ?? "";
  }

  set raw(value) {
    if (value == null || value.length == 0) {
      this._base64Value = undefined;
    } else {
      this._base64Value = value;
    }
    this.setNumericFromBase64(value);
    this.fire();
  }

  _numericValue?: string;
  get numeric() {
    return this._numericValue;
  }

  set numeric(value) {
    if (value == null || value.length == 0) {
      this._numericValue = undefined;
      this._base64Value = undefined;      
      this.fire();
      return;
    }
    this._numericValue = value;    
    try {
      let num = BigInt(value);
      this._base64Value = hexToBase64(num.toString(16), true);
    } catch (err) {
      this._base64Value = value;
    }
    this.fire();
  }

  get warningNumeric(): string | undefined {
    if(this._numericValue == null) {
      return undefined;
    }
    try {
      let num = BigInt(this._numericValue);
      let str = num.toString(this._numericValue.startsWith("0x") ? 16 : 10);
      let com = this._numericValue.trim();
      if(com.startsWith("0x")){
        com = com.substring(2);
      }
      while(com.length > 1 && com.startsWith("0")) {
        com = com.substring(1);
      }
      if(com.toLowerCase() == str.toLowerCase()) {
        return undefined;
      }
    } catch (err) {  
    }
    return "Invalid number.";
  }
  get warningRaw(): string | undefined {
    if (this._base64Value == null) {
      return undefined;
    }
    try {
      if (btoa(atob(this._base64Value)) == this._base64Value) {
        return undefined;
      }
    } catch (err) {
    }
    return "Invalid Base64 string.";
  }

  get safeNum() {
    if(this._numericValue == null) {
      return "";
    }
    try {
      let str = BigInt(this._numericValue).toString(this._numericValue.startsWith("0x") ? 10 : 16);
      if(!this._numericValue.startsWith("0x")){
        str = "0x" + str;
      }
      return str;
    } catch (err) {
      return this._numericValue;
    }
  }

}

