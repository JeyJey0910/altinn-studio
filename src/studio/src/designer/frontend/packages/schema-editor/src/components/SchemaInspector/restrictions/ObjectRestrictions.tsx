import React from 'react';
import { RestrictionItemProps } from '../ItemRestrictions';
import { FieldType, getRestrictions } from '@altinn/schema-model';
import { RestrictionField } from '../RestrictionField';
import { getTranslation } from '../../../utils/language';
import { Divider } from '../Divider';

export function ObjectRestrictions({ restrictions, path, language, onChangeRestrictionValue }: RestrictionItemProps) {
  const defaults = getRestrictions(FieldType.Object);
  return defaults?.length ? (
    <>
      <Divider />
      {defaults?.map((key) => (
        <RestrictionField
          key={key}
          path={path}
          label={getTranslation(key, language)}
          value={restrictions[key] ?? ''}
          keyName={key}
          readOnly={false}
          onChangeValue={onChangeRestrictionValue}
        />
      ))}
    </>
  ) : null;
}
