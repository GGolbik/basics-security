import { KeyValue } from "@angular/common";
import { Pipe, PipeTransform } from "@angular/core";


@Pipe({ name: 'enumkeyvalue' })
export class EnumKeyValuePipe implements PipeTransform {
  transform(value: any, none: boolean = true): KeyValue<number, string>[] {
    let keys = Object.keys(value).filter((v) => {
      return !isNaN(Number(+v));
    }).map((key) => Number(key));
    let ret = [] as KeyValue<number, string>[];
    keys.forEach((key) => {
      if (!none && value[key] == 'None') {
        return;
      }
      ret.push({
        key: key,
        value: value[key]
      } as KeyValue<number, string>)
    });
    return ret;
  }
}
