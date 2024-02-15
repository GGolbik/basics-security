import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CrlEntryComponent } from './crl-entry.component';

describe('CrlEntryComponent', () => {
  let component: CrlEntryComponent;
  let fixture: ComponentFixture<CrlEntryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CrlEntryComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CrlEntryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
