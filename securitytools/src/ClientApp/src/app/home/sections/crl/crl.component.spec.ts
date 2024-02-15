import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CrlComponent } from './crl.component';

describe('CrlComponent', () => {
  let component: CrlComponent;
  let fixture: ComponentFixture<CrlComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CrlComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CrlComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
