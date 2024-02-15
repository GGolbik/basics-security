import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelfCertificateConfigComponent } from './self-certificate-config.component';

describe('SelfCertificateConfigComponent', () => {
  let component: SelfCertificateConfigComponent;
  let fixture: ComponentFixture<SelfCertificateConfigComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SelfCertificateConfigComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SelfCertificateConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
