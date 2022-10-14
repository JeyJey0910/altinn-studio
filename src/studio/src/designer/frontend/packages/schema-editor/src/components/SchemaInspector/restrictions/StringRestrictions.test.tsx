import React from 'react';
import { render, screen } from '@testing-library/react';
import { StringRestrictions } from './StringRestrictions';
import { StrRestrictionKeys } from '@altinn/schema-model';

test('StringRestrictions should render correctly', async () => {
  const onChangeRestrictionValue = jest.fn();
  const path = '#/properties/xxsfds';
  render(
    <StringRestrictions
      onChangeRestrictionValue={onChangeRestrictionValue}
      language={{}}
      path={path}
      readonly={false}
      restrictions={[]}
    />,
  );
  Object.values(StrRestrictionKeys).forEach((text) => expect(screen.getByLabelText(text)).toBeDefined());
});
