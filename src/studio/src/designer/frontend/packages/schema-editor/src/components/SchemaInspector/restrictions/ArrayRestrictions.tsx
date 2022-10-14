import React from 'react';
import { RestrictionItemProps } from '../ItemRestrictions';
import { ArrRestrictionKeys } from '@altinn/schema-model';
import { RestrictionField } from '../RestrictionField';
import { getTranslation } from '../../../utils/language';
import { Divider } from '../../common/Divider';

export function ArrayRestrictions({ restrictions, language, path, onChangeRestrictionValue }: RestrictionItemProps) {
  return (
    <>
      <Divider />
      {Object.values(ArrRestrictionKeys).map((key: string) => (
        <RestrictionField
          key={key}
          path={path}
          label={getTranslation(key, language)}
          value={restrictions[key]}
          keyName={key}
          readOnly={false}
          onChangeValue={onChangeRestrictionValue}
        />
      ))}
    </>
  );
}
