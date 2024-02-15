import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CrlValidityComponent } from './crl-validity.component';

describe('CrlValidityComponent', () => {
  let component: CrlValidityComponent;
  let fixture: ComponentFixture<CrlValidityComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CrlValidityComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CrlValidityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
