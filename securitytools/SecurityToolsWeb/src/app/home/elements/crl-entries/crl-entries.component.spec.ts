import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CrlEntriesComponent } from './crl-entries.component';

describe('CrlEntriesComponent', () => {
  let component: CrlEntriesComponent;
  let fixture: ComponentFixture<CrlEntriesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CrlEntriesComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CrlEntriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
