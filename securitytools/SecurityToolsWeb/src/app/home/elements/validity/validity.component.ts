import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ConfigValidity, MY_FORMATS } from '../../../services/models';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from '@angular/material-moment-adapter';

@Component({
  selector: 'app-validity',
  templateUrl: './validity.component.html',
  styleUrls: ['./validity.component.css'],
  providers: [
    // `MomentDateAdapter` can be automatically provided by importing `MomentDateModule` in your
    // application's root module. We provide it at the component level here, due to limitations of
    // our example generation script.
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
    },

    {provide: MAT_DATE_FORMATS, useValue: MY_FORMATS},
  ],
})
export class ValidityComponent {
  notBefore?: Date;
  notAfter?: Date;

  @Input() set config(config: ConfigValidity | undefined) {
    this.notBefore = config?.notBefore == null ? undefined : new Date(config.notBefore);
    this.notAfter = config?.notAfter == null ? undefined : new Date(config.notAfter);
  }

  @Output() configChange = new EventEmitter<ConfigValidity | undefined>();

  fire() {
    let config = {} as ConfigValidity;
    config.notBefore = this.notBefore?.toISOString();
    config.notAfter = this.notAfter?.toISOString();
    if (config.notBefore == null && config.notAfter == null) {
      this.configChange.emit(undefined);
    } else {
      this.configChange.emit(config);
    }
  }
}
