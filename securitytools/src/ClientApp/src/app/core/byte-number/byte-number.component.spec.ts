import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ByteNumberComponent } from './byte-number.component';

describe('ByteNumberComponent', () => {
  let component: ByteNumberComponent;
  let fixture: ComponentFixture<ByteNumberComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ByteNumberComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ByteNumberComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
