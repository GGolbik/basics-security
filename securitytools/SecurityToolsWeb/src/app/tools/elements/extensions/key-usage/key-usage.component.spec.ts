import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KeyUsageComponent } from './key-usage.component';

describe('KeyUsageComponent', () => {
  let component: KeyUsageComponent;
  let fixture: ComponentFixture<KeyUsageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ KeyUsageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(KeyUsageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
