import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ConfigCrlValidity, MY_FORMATS } from '../../services/models';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from '@angular/material-moment-adapter';

@Component({
  selector: 'app-crl-validity',
  templateUrl: './crl-validity.component.html',
  styleUrls: ['./crl-validity.component.css'],
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
export class CrlValidityComponent {
  thisUpdate?: Date;
  nextUpdate?: Date;

  @Input() set config(config: ConfigCrlValidity | undefined) {
    this.thisUpdate = config?.thisUpdate == null ? undefined : new Date(config.thisUpdate);
    this.nextUpdate = config?.nextUpdate == null ? undefined : new Date(config.nextUpdate);
  }

  @Output() configChange = new EventEmitter<ConfigCrlValidity | undefined>();

  fire() {
    let config = {} as ConfigCrlValidity;
    config.thisUpdate = this.thisUpdate?.toISOString();
    config.nextUpdate = this.nextUpdate?.toISOString();
    if (config.thisUpdate == null && config.nextUpdate == null) {
      this.configChange.emit(undefined);
    } else {
      this.configChange.emit(config);
    }
  }
}
