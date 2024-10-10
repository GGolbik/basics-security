import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExtendedKeyUsageComponent } from './extended-key-usage.component';

describe('ExtendedKeyUsageComponent', () => {
  let component: ExtendedKeyUsageComponent;
  let fixture: ComponentFixture<ExtendedKeyUsageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ExtendedKeyUsageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExtendedKeyUsageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
