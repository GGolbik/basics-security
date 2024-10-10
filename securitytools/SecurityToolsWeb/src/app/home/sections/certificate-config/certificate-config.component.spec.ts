import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CertificateConfigComponent } from './certificate-config.component';

describe('CertificateConfigComponent', () => {
  let component: CertificateConfigComponent;
  let fixture: ComponentFixture<CertificateConfigComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CertificateConfigComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CertificateConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
