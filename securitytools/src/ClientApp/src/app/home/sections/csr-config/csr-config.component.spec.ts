import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CsrConfigComponent } from './csr-config.component';

describe('CsrConfigComponent', () => {
  let component: CsrConfigComponent;
  let fixture: ComponentFixture<CsrConfigComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CsrConfigComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CsrConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
