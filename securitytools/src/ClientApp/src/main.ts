import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

export function getBaseApiUrl() {
  return document.getElementsByTagName('base')[0].href + "api/"; // "api/v1.0/";
}

const providers = [
  { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
  { provide: 'BASE_API_URL', useFactory: getBaseApiUrl, deps: [] }
];

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic(providers).bootstrapModule(AppModule)
  .catch(err => console.log(err));

export function toLocalISOString(d?: Date): string {
  if (d == null) {
    return '';
  }
  var off = d.getTimezoneOffset();
  return new Date(d.getFullYear(), d.getMonth(), d.getDate(), d.getHours(), d.getMinutes() - off, d.getSeconds(), d.getMilliseconds()).toISOString().slice(0, -1);
}

export function arrayBufferToBase64(buffer: any) {
  var binary = '';
  var bytes = new Uint8Array(buffer);
  var len = bytes.byteLength;
  for (var i = 0; i < len; i++) {
    binary += String.fromCharCode(bytes[i]);
  }
  return window.btoa(binary);
}

export function exportJson(value: any) {
  var a = document.createElement("a");
  //let blob = base64ToArrayBuffer(cert.rawData ?? "");
  let blob = JSON.stringify(value);
  a.href = URL.createObjectURL(new Blob([blob], { type: "application.json" }));
  a.download = "keypair.conf.json";
  a.click();
  URL.revokeObjectURL(a.href);
}

export function hexToBase64(str:string, fill = false) {
  if(fill && (str.length % 2 != 0)) {
    str = "0" + str;
  }
  var result: number[]  = [];
  while (str.length >= 2) { 
      result.push(parseInt(str.substring(0, 2), 16));
      str = str.substring(2, str.length);
  }
  return btoa(String.fromCodePoint(...result));
}

export function base64ToHex(str:string) {
  const raw = atob(str);
  let result = '';
  for (let i = 0; i < raw.length; i++) {
    const hex = raw.charCodeAt(i).toString(16);
    result += (hex.length === 2 ? hex : '0' + hex);
  }
  return result.toUpperCase();
}

export function renameFile(file: File, name: string) : File {
  let data = new FormData();
  data.append("file", file, name);
  return data.get("file") as File;
}

/**
 * Converts a file size from bytes into a human readable file size.
 * 
 * @param size The size in bytes.
 * @param fractionDigits Number of digits after the decimal point.
 * @returns A human readable file size.
 */
export function toHumanSize(size: number, fractionDigits = 2, locale = false): string {
  if(fractionDigits < 0) {
    fractionDigits = 0;
  } else if (fractionDigits > 20) {
    fractionDigits = 20;
  }
  if (size <= 0) {
    return "0 Byte";
  }
  if(size < 1024) {
    return (locale ? size.toLocaleString() : size) + " Byte";
  }
  size /= 1024;  
  if(size < 1024) {
    return (locale ? Number(size.toFixed(2)).toLocaleString() : size.toFixed(2))+ " KB";
  }
  size /= 1024;  
  if(size < 1024) {
    return (locale ? Number(size.toFixed(2)).toLocaleString() : size.toFixed(2)) + " MB";
  }
  size /= 1024;  
  if(size < 1024) {
    return (locale ? Number(size.toFixed(2)).toLocaleString() : size.toFixed(2)) + " GB";
  }
  size /= 1024;
  return (locale ? Number(size.toFixed(2)).toLocaleString() : size.toFixed(2)) + " TB";
}

export function base64ToByteLength(base64?: string) : number {
  if(base64 == null) {
    return -1;
  }
  try {
    atob(base64);
  } catch {
    return -1;
  }
  let numberOfPaddingCharacters = 0;
  if(base64.lastIndexOf("==") >= 0) {
    numberOfPaddingCharacters = 2;
  } else if(base64.lastIndexOf("=") >= 0) {
    numberOfPaddingCharacters = 1;
  }
  return (3 * (base64.length / 4)) - (numberOfPaddingCharacters);
}

export function replaceAll(str: string, find: string, replace: string) {
  var escapedFind = find.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
  return str.replace(new RegExp(escapedFind, 'g'), replace);
}

export function escapeHtml(unsafe: string) {
  return unsafe
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#039;");
}
